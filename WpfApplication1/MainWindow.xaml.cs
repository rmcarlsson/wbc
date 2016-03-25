using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using GFCalc.Domain;
using GFCalc.Repos;
using Grainsim;
using Grainsim.Domain;
using System.Xml.Serialization;
using System.IO;
using System.Xml;
using BSImport;
using Grainsim.BeersmithImporterWizard;

namespace GFCalc
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<GristPart> Grist { set; get; }
        public ObservableCollection<HopBoilAddition> BoilHops { set; get; }
        public ObservableCollection<MashProfileStep> MashProfileList { get; set; }
        public ObservableCollection<OtherIngredient> OtherIngredientsList { get; set; }


        public double BatchSize { get; set; }
        public double PreBoilVolume { get; set; }
        public double OriginalGravity { get; set; }
        public double PreBoilGravity { get; set; }
        public double MashEfficieny { get; set; }
        public double GrainBillSize { get; set; }
        public int BoilTime { set; get; }
        public double TopUpMashWater { set; get; }


        private FermentableRepository MaltRepo;
        private HopsRepository HopsRepo;

        public MainWindow()
        {
            InitializeComponent();

            HopsRepo = new HopsRepository();
            MaltRepo = new FermentableRepository();

            Grist = new ObservableCollection<GristPart>();
            MaltsListView.ItemsSource = Grist;
            BoilHops = new ObservableCollection<HopBoilAddition>();
            HopsListView.ItemsSource = BoilHops;

            MashProfileList = new ObservableCollection<MashProfileStep>();
            MashStepListView.ItemsSource = MashProfileList;

            OtherIngredientsList = new ObservableCollection<OtherIngredient>();
            OtherIngredientsListView.ItemsSource = OtherIngredientsList;

            BatchSize = 25;
            OriginalGravity = 1.05;
            BoilTime = 60;
            TopUpMashWater = 0;

            updateGuiTextboxes();
            recalulateVolumes();

        }

        private void recalulateVolumes()
        {
            if (PreBoilVolumeCheckbox.IsChecked == false)
            {
                PreBoilVolumeTextBox.Text = GrainfatherCalculator.CalcPreBoilVolume(BatchSize, BoilTime).ToString();
                PreBoilVolumeTextBox.IsEnabled = false;
                PreBoilVolumeTextBox.IsReadOnly = true;

            }
            else
            {
                PreBoilVolumeTextBox.IsEnabled = true;
                PreBoilVolumeTextBox.IsReadOnly = false;
            }

            if (VolumeInFermentorCheckbox.IsChecked == false)

            {
                VolumeInFermentorTextBox.Text = BatchSize.ToString();
                VolumeInFermentorTextBox.IsEnabled = false;
                VolumeInFermentorTextBox.IsReadOnly = true;
            }
            else
            {
                VolumeInFermentorTextBox.IsEnabled = true;
                VolumeInFermentorTextBox.IsReadOnly = false;
            }





        }

        private void addGrains_Click(object sender, RoutedEventArgs e)
        {
            var sum = Grist.Sum(g => g.Amount);
            var sw = new SelectGrain(MaltRepo, sum);
            sw.ShowDialog();
            Grist.Add(sw.Result);

            recalculateGrainBill();
        }

        private void listView_KeyDown(object sender, KeyEventArgs e)
        {
            if (Key.Delete == e.Key)
            {

                foreach (GristPart listViewItem in ((ListView)sender).SelectedItems)
                {
                    Grist.Remove(listViewItem);
                    MessageBox.Show(String.Format("Shit, want to remove {0}. That is {1} away", listViewItem.FermentableAdjunct.Name, listViewItem.Amount));
                    break;
                }
            }
        }



        private void recalculateGrainBill()
        {
            var sum = Grist.Sum(g => g.Amount);
            if (sum != 100)
            {
                sum = -1;
                GrainBillSize = 0;
            }
            else
            {
                //double boilOffVolume = GrainfatherCalculator.CalcBoilOffVolume(BatchSize, BoilTime);
                GrainBillSize = GravityAlorithms.GetGrainBillWeight(OriginalGravity, BatchSize, Grist.ToList(), null, GrainfatherCalculator.MashEfficiency);
            }
            var l = new List<GristPart>();
            foreach (var grain in Grist)
            {
                if (grain.Stage.Equals("Cold steep"))
                {
                    grain.AmountGrams = ColdSteep.GetGrainBillSize(grain, GrainBillSize);
                }
                if (grain.Stage.Equals("Mash"))
                {
                    grain.AmountGrams = Math.Round((grain.Amount * GrainBillSize) / sum);
                }
                if (grain.Stage.Equals("Fermentation"))
                {
                    grain.AmountGrams = Math.Round(((grain.Amount * GrainBillSize) / sum) * (BatchSize / (BatchSize + GrainfatherCalculator.GRAINFATHER_BOILER_TO_FERMENTOR_LOSS)));
                }

                l.Add(grain);
            }

            foreach (var grain in l)
            {
                Grist.Remove(grain);
                Grist.Add(grain);
            }

            double topUpVolume = 0;
            if (GrainBillSize > GrainfatherCalculator.SMALL_GRAINBILL || GrainBillSize == 0)
            {
                TopUpMashWaterVolumeTextBox.Visibility = Visibility.Hidden;
                TopUpMashWaterVolumeLabel.Visibility = Visibility.Hidden;

            }
            else
            {
                TopUpMashWaterVolumeTextBox.Visibility = Visibility.Visible;
                TopUpMashWaterVolumeLabel.Visibility = Visibility.Visible;
                topUpVolume = TopUpMashWater;

            }
            try
            {
                var swv = GrainfatherCalculator.CalcSpargeWaterVolume(GrainBillSize,
                    (BatchSize + GrainfatherCalculator.GRAINFATHER_BOILER_TO_FERMENTOR_LOSS + GrainfatherCalculator.CalcBoilOffVolume(BatchSize, BoilTime)),
                    topUpVolume);
                if (swv < 0)
                    swv = 0;
                SpargeWaterVolumeLabel.Content = String.Format("Sparge water volume [L]: {0:0.#}", swv);

                var mwv = GrainfatherCalculator.CalcMashVolume(Mash.CalculateMashGrainBillSize(Grist.ToList(), GrainBillSize));
                if (mwv < 0)
                    mwv = 0;
                MashWaterVolumeLabel.Content = String.Format("Mash water volume [L]: {0:0.#}", mwv);
            }
            catch (ArgumentException e)
            {
                MessageBox.Show(e.Message);
            }
            foreach (GridViewColumn c in MaltsGridView.Columns)
            {
                c.Width = 0; //set it to no width
                c.Width = double.NaN; //resize it automatically
            }

            ColorLabel.Content = String.Format("Color [ECB]: {0}", ColorAlgorithms.CalculateColor(Grist.ToList(), BatchSize));

            recalulateVolumes();
        }


        private void ExpectedOriginalGravityTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ExpectedOriginalGravityTextBox = (TextBox)(sender);
            double val = 0;

            if (Grist != null &&
                Double.TryParse(ExpectedOriginalGravityTextBox.Text, out val))
            {
                OriginalGravity = val;
                recalculateGrainBill();
            }
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            var w = new SelectHops(HopsRepo, BoilTime);
            w.ShowDialog();
            if (w.hop != null)
            {
                BoilHops.Add(w.hop);
                recalculateIbu();
            }
        }

        private void MaltsListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

            if (MaltsListView.SelectedIndex >= Grist.Count() || MaltsListView.SelectedIndex < 0)
                return;

            double sum = Grist.Sum(g => g.Amount);
            sum -= Grist.ToArray()[MaltsListView.SelectedIndex].Amount;
            var sw = new SelectGrain(MaltRepo, sum, Grist.ToArray()[MaltsListView.SelectedIndex]);
            sw.ShowDialog();
            if (sw.Result != null)
            {
                Grist.Remove((GristPart)MaltsListView.SelectedItem);
                Grist.Add(sw.Result);
                recalculateGrainBill();
            }
        }

        private void BatchSizeVolumeTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            BatchSizeVolumeTextBox = (TextBox)(sender);
            double val = 0;
            if (Grist != null &&
                 Double.TryParse(BatchSizeVolumeTextBox.Text, out val))
            {
                if (val > (GrainfatherCalculator.GRAINFATHER_MAX_PREBOILVOLUME - GrainfatherCalculator.CalcBoilOffVolume(val, BoilTime)))
                {
                    MessageBox.Show("Batch size is to big. Please reduce it");
                    return;
                }

                BatchSize = val;
                recalculateGrainBill();
            }
        }


        private void HopsListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

            if (HopsListView.SelectedIndex >= BoilHops.Count() || HopsListView.SelectedIndex < 0)
                return;

            var w = new SelectHops(HopsRepo, BoilHops.ToArray()[HopsListView.SelectedIndex]);
            w.ShowDialog();
            if (w.hop != null)
            {
                BoilHops.Remove((HopBoilAddition)HopsListView.SelectedItem);
                BoilHops.Add(w.hop);
                recalculateIbu();
            }


        }

        private void BoilTimeTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            BoilTimeTextBox = (TextBox)(sender);
            int val = 0;
            if (Grist != null &&
                 Int32.TryParse(BatchSizeVolumeTextBox.Text, out val))
            {
                BoilTime = val;
                recalculateIbu();
            }

        }

        private void recalculateIbu()
        {
            IbuLabel.Content = string.Format("IBU: {0}", IbuAlgorithms.CalcIbu(BoilHops.Where(x => x.Stage.Equals("Boil")), OriginalGravity, BatchSize));
        }

        private void TopUpMashWaterVolumeTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TopUpMashWaterVolumeTextBox = (TextBox)(sender);
            double val = 0;
            if (Grist != null &&
                 Double.TryParse(TopUpMashWaterVolumeTextBox.Text, out val))
            {
                TopUpMashWater = val;
                recalculateGrainBill();
            }
        }


        ///////////////////////////////////////////////////////////////////////
        //
        // Mash profile handing
        //
        ///////////////////////////////////////////////////////////////////////

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void AddMashStepButton_Click(object sender, RoutedEventArgs e)
        {
            double temp;
            if (!double.TryParse(MashStepTempTextBox.Text, out temp))
                return;

            int time;
            if (!int.TryParse(MashStepTimeTextBox.Text, out time))
                return;

            var mps = new MashProfileStep();
            mps.Temperature = temp;
            mps.Time = time;
            MashProfileList.Add(mps);
            AddMashStepButton.Content = "Add";

        }

        private void MasStepListView_KeyDown(object sender, KeyEventArgs e)
        {

            if (Key.Delete == e.Key)
            {
                foreach (MashProfileStep listViewItem in ((ListView)sender).SelectedItems)
                {
                    MashProfileList.Remove(listViewItem);
                    break;
                }
            }
        }

        private void MasStepListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MashStepTempTextBox.Text = MashProfileList.ToArray()[MashStepListView.SelectedIndex].Temperature.ToString();
            MashStepTimeTextBox.Text = MashProfileList.ToArray()[MashStepListView.SelectedIndex].Time.ToString();
            MashProfileList.Remove((MashProfileStep)MashStepListView.SelectedItem);
            AddMashStepButton.Content = "Update";

        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            var r = new Recepie();
            r.Fermentables = Grist.ToList();
            r.BoilHops = BoilHops.ToList();
            r.MashProfile = MashProfileList.ToList();
            r.Name = NameTextBox.Text;
            r.BatchSize = BatchSize;
            r.OriginalGravity = OriginalGravity;
            r.BoilTime = BoilTime;


            // Create OpenFileDialog 
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();



            // Set filter for file extension and default file extension 
            dlg.DefaultExt = ".xml";
            dlg.Filter = "Grainfather recepie files|*.xml";
            dlg.CheckFileExists = false;
            dlg.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            dlg.FileName = NameTextBox.Text;

            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();


            if (result != true)
                return;

            XmlSerializer serializer = new XmlSerializer(typeof(Recepie));
            FileStream saveStream = new FileStream(dlg.FileName, FileMode.OpenOrCreate, FileAccess.Write);
            serializer.Serialize(saveStream, r);
            saveStream.Close();



        }

        private void MenuItem_Open(object sender, RoutedEventArgs e)
        {

            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();



            // Set filter for file extension and default file extension 
            dlg.DefaultExt = ".xml";
            dlg.Filter = "Grainfather recepie files|*.xml";
            dlg.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);


            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();


            if (result != true)
                return;

            if (!TryOpenFile(dlg.FileName))
            {
                MessageBox.Show("Unable to parse recepie");
                importBeersmithRecipe(dlg.FileName);
            }


        }

        private void importBeersmithRecipe(string aRecipeFileName)
        {
            var bsiw = new TCW(aRecipeFileName, MaltRepo, HopsRepo);
            bsiw.ShowDialog();
            var r = bsiw.ImportedRecipe;

        }

        private bool TryOpenFile(string aFileName)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Recepie));
            FileStream loadStream = new FileStream(aFileName, FileMode.Open, FileAccess.Read);
            XmlReader reader = new XmlTextReader(loadStream);
            if (!serializer.CanDeserialize(reader))
                return false;

            Recepie loadedObject = (Recepie)serializer.Deserialize(reader);
            loadStream.Close();
            Grist.Clear();
            foreach (GristPart g in loadedObject.Fermentables)
                Grist.Add(g);

            BoilHops.Clear();
            foreach (HopBoilAddition h in loadedObject.BoilHops)
                BoilHops.Add(h);

            MashProfileList.Clear();
            foreach (MashProfileStep mps in loadedObject.MashProfile)
                MashProfileList.Add(mps);


            BatchSize = loadedObject.BatchSize;
            OriginalGravity = loadedObject.OriginalGravity;
            BoilTime = loadedObject.BoilTime;
            TopUpMashWater = loadedObject.TopUpMashWater;
            NameTextBox.Text = loadedObject.Name;

            recalculateGrainBill();
            recalculateIbu();

            updateGuiTextboxes();

            return true;
        }

        private void updateGuiTextboxes()
        {
            BatchSizeVolumeTextBox.Text = BatchSize.ToString();
            ExpectedOriginalGravityTextBox.Text = OriginalGravity.ToString();
            BoilTimeTextBox.Text = BoilTime.ToString();
            TopUpMashWaterVolumeTextBox.Text = TopUpMashWater.ToString();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void MenuItem_Click_3(object sender, RoutedEventArgs e)
        {
            var sw = new AlterMaltsWindow(MaltRepo);
            sw.ShowDialog();

        }

        private void MenuItem_Add_Hops(object sender, RoutedEventArgs e)
        {
            var sw = new AlterHopsWindow(HopsRepo);
            sw.ShowDialog();
        }

        private void MenuItem_File_Print(object sender, RoutedEventArgs e)
        {
            PrintDialog pDialog = new PrintDialog();
            pDialog.PageRangeSelection = PageRangeSelection.AllPages;
            pDialog.UserPageRangeEnabled = true;

            // Display the dialog. This returns true if the user presses the Print button.
            Nullable<Boolean> print = pDialog.ShowDialog();
            if (print == true)
            {
                FlowDocument doc = new FlowDocument(new Paragraph(new Run("Some text goes here")));
                doc.Name = "FlowDoc";


                // Create IDocumentPaginatorSource from FlowDocument
                IDocumentPaginatorSource idpSource = doc;

                // Call PrintDocument method to send document to printer
                pDialog.PrintDocument(idpSource.DocumentPaginator, "Hello WPF Printing.");

                //XpsDocument xpsDocument = new XpsDocument("C:\\FixedDocumentSequence.xps", FileAccess.ReadWrite);
                //FixedDocumentSequence fixedDocSeq = xpsDocument.GetFixedDocumentSequence();
                //pDialog.PrintDocument(fixedDocSeq.DocumentPaginator, "Test print job");
            }

        }

        private void VolumeInFermentorCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            recalulateVolumes();
        }

        private void PreBoilVolumeCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            recalulateVolumes();
        }

        private void AddOtherIngredientsButton_Click(object sender, RoutedEventArgs e)
        {
            var w = new AddOtherIngredients();
            w.ShowDialog();
            if (w.Result != null)
            {
                OtherIngredientsList.Add(w.Result);
            }
        }
    }

}

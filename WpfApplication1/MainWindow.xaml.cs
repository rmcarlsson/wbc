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
using System.Text;
using System.Windows.Media;

namespace GFCalc
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<GristPart> Grist { set; get; }
        public ObservableCollection<HopAddition> BoilHops { set; get; }
        public ObservableCollection<MashProfileStep> MashProfileList { get; set; }
        public ObservableCollection<OtherIngredient> OtherIngredientsList { get; set; }


        public double PreBoilVolume { get; set; }
        public double OriginalGravity { get; set; }
        public double PreBoilGravity { get; set; }
        public double MashEfficieny { get; set; }
        public int GrainBillSize { get; set; }
        public int BoilTime { set; get; }
        public double TopUpMashWater { set; get; }
        public double PreBoilRemovedVolume { get; private set; }

        private BrewVolumes Volumes;

        private FermentableRepository MaltRepo;
        private HopsRepository HopsRepo;

        public MainWindow()
        {
            InitializeComponent();

            HopsRepo = new HopsRepository();
            MaltRepo = new FermentableRepository();

            Grist = new ObservableCollection<GristPart>();
            MaltsListView.ItemsSource = Grist;
            BoilHops = new ObservableCollection<HopAddition>();
            HopsListView.ItemsSource = BoilHops;

            MashProfileList = new ObservableCollection<MashProfileStep>();
            MashStepListView.ItemsSource = MashProfileList;

            OtherIngredientsList = new ObservableCollection<OtherIngredient>();
            OtherIngredientsListView.ItemsSource = OtherIngredientsList;

            OriginalGravity = 1.05;
            BoilTime = 60;

            Volumes = new BrewVolumes();
            Volumes.BoilOffLoss = GrainfatherCalculator.CalcBoilOffVolume(BoilTime);
            Volumes.FinalBatchVolume = 25;
            Volumes.BoilerToFermentorLoss = GrainfatherCalculator.GRAINFATHER_BOILER_TO_FERMENTOR_LOSS;
            Volumes.PreBoilTapOff = 0;

            TopUpMashWater = 0;

            updateGuiTextboxes();

        }


        #region MaltsListView event handlers
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

        private void MaltsListView_KeyDown(object sender, KeyEventArgs e)
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

        private void addGrains_Click(object sender, RoutedEventArgs e)
        {
            var sum = Grist.Sum(g => g.Amount);
            var sw = new SelectGrain(MaltRepo, sum);
            sw.ShowDialog();

            if (sw.Result != null)
            {
                Grist.Add(sw.Result);
                recalculateGrainBill();
            }
        }
        #endregion

        #region HopsListView event handlers
        private void HopsListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

            if (HopsListView.SelectedIndex >= BoilHops.Count() || HopsListView.SelectedIndex < 0)
                return;

            var w = new SelectHops(HopsRepo, BoilHops.ToArray()[HopsListView.SelectedIndex]);
            w.ShowDialog();
            if (w.hop != null)
            {
                BoilHops.Remove((HopAddition)HopsListView.SelectedItem);
                BoilHops.Add(w.hop);
                recalculateIbu();
            }


        }
        private void HopsListView_KeyDown(object sender, KeyEventArgs e)
        {
            if (HopsListView.SelectedIndex >= BoilHops.Count() || HopsListView.SelectedIndex < 0)
                return;

            if (Key.Delete == e.Key)
            {
                HopAddition h = (HopAddition)HopsListView.SelectedItem;
                BoilHops.Remove(h);
                recalculateIbu();

                MessageBox.Show(String.Format("Shit, want to remove {0}. That is {1} away", h.Hop.Name, h.Amount));
            }
        }

        private void AddHopsButton_Click(object sender, RoutedEventArgs e)
        {
            var w = new SelectHops(HopsRepo, BoilTime);
            w.ShowDialog();
            if (w.hop != null)
            {
                BoilHops.Add(w.hop);
                var ol = BoilHops.OrderBy(x => x).ToList();
                BoilHops.Clear();
                foreach (HopAddition h in ol)
                    BoilHops.Add(h);
                recalculateIbu();
            }
        }

        #endregion

        #region MashStepListView event handlers
        private void MashStepListView_KeyDown(object sender, KeyEventArgs e)
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

        private void MashStepListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MashStepTempTextBox.Text = MashProfileList.ToArray()[MashStepListView.SelectedIndex].Temperature.ToString();
            MashStepTimeTextBox.Text = MashProfileList.ToArray()[MashStepListView.SelectedIndex].Time.ToString();
            MashProfileList.Remove((MashProfileStep)MashStepListView.SelectedItem);
            AddMashStepButton.Content = "Update";

        }
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
            var ol = MashProfileList.OrderBy(x => x.Temperature).ToList();
            MashProfileList.Clear();
            foreach (MashProfileStep ms in ol)
            {
                MashProfileList.Add(ms);
            }

            AddMashStepButton.Content = "Add";

        }
        #endregion

        #region File menu event handlers
        private void MenuItem_FileSave(object sender, RoutedEventArgs e)
        {
            var r = new Recepie();
            r.Fermentables = Grist.ToList();
            r.BoilHops = BoilHops.ToList();
            r.MashProfile = MashProfileList.ToList();
            r.OtherIngredients = OtherIngredientsList.ToList();
            r.Name = NameTextBox.Text;
            r.BatchVolume = Volumes.TotalBatchVolume;
            r.OriginalGravity = OriginalGravity;
            r.BoilTime = BoilTime;


            // Create OpenFileDialog 
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();



            // Set filter for file extension and default file extension 
            dlg.DefaultExt = ".gsrx";
            dlg.Filter = "Grainfather recepie files (*.gsrx)|*.gsrx";
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

        private void MenuItem_FileOpen(object sender, RoutedEventArgs e)
        {

            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();



            // Set filter for file extension and default file extension 
            dlg.Filter = "Grainfather recepie files (*.gsrx)|*.gsrx|Beersmith2 file (*.bsmx)|*.bsmx";
            dlg.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);


            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();


            if (result != true)
                return;

            if (!TryOpenFile(dlg.FileName))
            {
                importBeersmithRecipe(dlg.FileName);
            }


        }

        private void importBeersmithRecipe(string aRecipeFileName)
        {
            var bsiw = new TCW(aRecipeFileName, MaltRepo, HopsRepo);
            bsiw.ShowDialog();
            var r = bsiw.ImportedRecepie;

            PopulateGUI(r);

        }

        private bool TryOpenFile(string aFileName)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Recepie));
            FileStream loadStream = new FileStream(aFileName, FileMode.Open, FileAccess.Read);
            XmlReader reader = new XmlTextReader(loadStream);
            if (!serializer.CanDeserialize(reader))
                return false;

            try {
                Recepie loadedObject = (Recepie)serializer.Deserialize(reader);
                PopulateGUI(loadedObject);

            }
            catch (Exception e)
            {
                MessageBox.Show("The recipe can be parsed.");
            }
            finally
            {
                loadStream.Close();
            }

            return true;
        }



        private void PopulateGUI(Recepie aRecepie)
        {

            Grist.Clear();
            foreach (GristPart g in aRecepie.Fermentables)
                Grist.Add(g);

            BoilHops.Clear();
            foreach (HopAddition h in aRecepie.BoilHops)
                BoilHops.Add(h);

            MashProfileList.Clear();
            foreach (MashProfileStep mps in aRecepie.MashProfile)
                MashProfileList.Add(mps);

            OtherIngredientsList.Clear();
            foreach (OtherIngredient o in aRecepie.OtherIngredients)
                OtherIngredientsList.Add(o);


            Volumes.FinalBatchVolume = aRecepie.BatchVolume;
            Volumes.PreBoilTapOff = aRecepie.PreBoilTapOffVolume;
            TopUpMashWater = aRecepie.TopUpMashWater;

            OriginalGravity = aRecepie.OriginalGravity;

            BoilTime = aRecepie.BoilTime;

            NameTextBox.Text = aRecepie.Name;

            recalculateGrainBill();

            recalculateIbu();

            updateGuiTextboxes();
        }


        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        private void MenuItem_FileExit(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void MenuItem_FilePrint(object sender, RoutedEventArgs e)
        {
            PrintDialog pDialog = new PrintDialog();
            pDialog.PageRangeSelection = PageRangeSelection.AllPages;
            pDialog.UserPageRangeEnabled = true;

            // Display the dialog. This returns true if the user presses the Print button.
            Nullable<Boolean> print = pDialog.ShowDialog();
            if (print == true)
            {
                FlowDocument doc = new FlowDocument();
                doc.FontFamily = new FontFamily("Courier");
                doc.FontSize = 11;

                // TODO: Why do I need to add ColumnWidth to get the margin of the print document correct?
                doc.PageWidth = 10000;
                doc.MinPageWidth = 10000;
                doc.ColumnWidth = 10000;


                #region Print mash part
                StringBuilder strbcs = new StringBuilder();
                StringBuilder strbmash = new StringBuilder();
                StringBuilder strbferm = new StringBuilder();
                foreach (GristPart g in Grist)
                {

                    switch (g.Stage)
                    {
                        case FermentableStage.ColdSteep:
                            strbcs.Append(g.ToString() + "\n");
                            break;
                        case FermentableStage.Mash:
                            strbmash.Append(g.ToString() + "\n");
                            break;
                        case FermentableStage.Fermentor:
                            strbferm.Append(g.ToString() + "\n");
                            break;
                        default:
                            break;
                    }
                }

                var mashHeading = new Paragraph(new Bold(new Run("Mash")));
                mashHeading.FontSize = 18;
                doc.Blocks.Add(mashHeading);

                if (Grist.Any(x => x.Stage == FermentableStage.ColdSteep))
                {
                    Paragraph pcs = new Paragraph();
                    pcs.Inlines.Add(new Bold(new Run("Cold steep, 24 before brew start:\n")));
                    pcs.Inlines.Add(new Run(strbcs.ToString()));
                    doc.Blocks.Add(pcs);
                }

                Paragraph pm = new Paragraph();
                pm.Inlines.Add(new Bold(new Run("Normal step mash:\n")));

                var mashGrainBillWeight = Grist.Where(x => x.Stage == FermentableStage.Mash).Sum(x => x.AmountGrams);
                var topUpVolume = 0d;

                pm.Inlines.Add(new Run(String.Format("Add {0:F1} liters of water to Grainfather for mashing\n",
                    GrainfatherCalculator.CalcMashVolume(mashGrainBillWeight))));
                pm.Inlines.Add(new Run("\nGrain bill:\n"));
                pm.Inlines.Add(new Run(strbmash.ToString()));
                doc.Blocks.Add(pm);


                StringBuilder strmp = new StringBuilder("");
                foreach (MashProfileStep mss in MashProfileList)
                {
                    strmp.Append(mss.ToString() + "\n");
                }


                Paragraph pmp = new Paragraph();
                pmp.Inlines.Add(new Bold(new Run("Mash profile:\n")));
                pmp.Inlines.Add(new Run(strmp.ToString()));

                pmp.Inlines.Add(new Run(String.Format("Sparge with {0:F1} liters of 78 C water\n",
                    GrainfatherCalculator.CalcSpargeWaterVolume(mashGrainBillWeight,
                    (Volumes.PreBoilVolume),
                    topUpVolume))));


                // Total volume points
                var totalBatchPoints = GravityAlorithms.GetPoints(OriginalGravity, Volumes.TotalBatchVolume);

                // Mash points
                var mashGravityPercent = Grist.Where(x => x.Stage == FermentableStage.Mash).Sum(x => x.Amount);
                var totalBatchMashPoints = (totalBatchPoints * ((double)(mashGravityPercent) / 100d));
                if (Volumes.PreBoilTapOff != 0)
                    totalBatchMashPoints += totalBatchMashPoints * (Volumes.PreBoilTapOff / Volumes.PreBoilVolume);

                pmp.Inlines.Add(new Run(String.Format("Pre-boil gravity [SG]: {0:F3}\n",
                    GravityAlorithms.GetGravity(totalBatchMashPoints, Volumes.PreBoilVolume))));

                doc.Blocks.Add(pmp);

                #endregion


                #region Boiling part
                var boilingHeader = (new Paragraph(new Bold(new Run("Boiling"))));
                boilingHeader.FontSize = 16;
                doc.Blocks.Add(boilingHeader);

                StringBuilder strbboil = new StringBuilder("");
                StringBuilder strbdry = new StringBuilder("");
                foreach (HopAddition g in BoilHops)
                {
                    switch (g.Stage)
                    {
                        case HopAdditionStage.Boil:
                            strbboil.Append(g.ToString() + "\n");
                            break;
                        case HopAdditionStage.Fermentation:
                            strbboil.Append(g.ToString() + "\n");
                            break;
                        default:
                            break;
                    }
                }
                doc.Blocks.Add(new Paragraph(new Run("Hop additions:")));
                Paragraph pbh = new Paragraph();
                pbh.Inlines.Add(new Run(strbboil.ToString()));



                var vm = Volumes.PreBoilVolume;
                var vcs = Volumes.ColdSteepVolume;

                foreach (GristPart g in Grist)
                {
                    if (g.Stage == FermentableStage.ColdSteep)
                        pbh.Inlines.Add(new Run(String.Format("Add the runnings of {0} to the boil 10 minutes before end\n", g.FermentableAdjunct.Name)));
                }

                pbh.Inlines.Add(new Run(String.Format("Expected post-boil gravity [SG]: {0:F3}\n", OriginalGravity)));

                doc.Blocks.Add(pbh);
                #endregion


                #region Print others part
                StringBuilder strbo = new StringBuilder("");
                foreach (OtherIngredient g in OtherIngredientsList)
                {
                    strbo.Append(String.Format("Add " + g.ToString() + "\n"));
                }
                if (OtherIngredientsList.Any())
                {
                    var othersHeading = new Paragraph(new Bold(new Run("Others")));
                    othersHeading.FontSize = 16;
                    doc.Blocks.Add(othersHeading);

                    Paragraph po = new Paragraph(new Run(strbo.ToString()));
                    doc.Blocks.Add(po);
                }
                #endregion


                doc.Name = "FlowDoc";
                doc.PageWidth = 10000;
                doc.MinPageWidth = 10000;

                // Create IDocumentPaginatorSource from FlowDocument
                IDocumentPaginatorSource idpSource = doc;

                // Call PrintDocument method to send document to printer
                pDialog.PrintDocument(idpSource.DocumentPaginator, NameTextBox.Text);

                //XpsDocument xpsDocument = new XpsDocument("C:\\FixedDocumentSequence.xps", FileAccess.ReadWrite);
                //FixedDocumentSequence fixedDocSeq = xpsDocument.GetFixedDocumentSequence();
                //pDialog.PrintDocument(fixedDocSeq.DocumentPaginator, "Test print job");
            }

        }

        #endregion

        #region Ingredients menu event handlers

        private void MenuItem_IngredientsAddMalts(object sender, RoutedEventArgs e)
        {
            var sw = new AlterMaltsWindow(MaltRepo);
            sw.ShowDialog();

        }

        private void MenuItem_Add_Hops(object sender, RoutedEventArgs e)
        {
            var sw = new AlterHopsWindow(HopsRepo);
            sw.ShowDialog();
        }

        #endregion

        #region Private methods

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
                // Total volume points
                var totalBatchPoints = GravityAlorithms.GetPoints(OriginalGravity, Volumes.TotalBatchVolume);

                // Mash points
                var mashGravityPercent = Grist.Where(x => x.Stage == FermentableStage.Mash).Sum(x => x.Amount);
                var totalBatchMashPoints = (totalBatchPoints * ((double)(mashGravityPercent) / 100d));
                if (Volumes.PreBoilTapOff != 0)
                    totalBatchMashPoints += totalBatchMashPoints * (Volumes.PreBoilTapOff / Volumes.PreBoilVolume);

                // Cold steep points
                var coldSteepGravityPercent = Grist.Where(x => x.Stage == FermentableStage.ColdSteep).Sum(x => x.Amount);
                var coldSteepPoints = (totalBatchPoints * ((double)(coldSteepGravityPercent) / 100d));


                Volumes.ColdSteepVolume = 0;

                var l = new List<GristPart>();
                foreach (var grain in Grist)
                {
                    if (grain.Stage == FermentableStage.ColdSteep)
                    {
                        grain.AmountGrams = GravityAlorithms.GetGrainWeight(totalBatchPoints * ((double)grain.Amount / 100d), grain.FermentableAdjunct.Potential);
                        Volumes.ColdSteepVolume += ColdSteep.GetColdSteepWaterContibution(grain.AmountGrams);
                    }
                    if (grain.Stage == FermentableStage.Mash)
                    {
                        grain.AmountGrams = GravityAlorithms.GetGrainWeight(totalBatchMashPoints * ((double)grain.Amount / 100d),
                            grain.FermentableAdjunct.Potential);
                    }
                    if (grain.Stage == FermentableStage.Fermentor)
                    {
                        var boilerLossPercent = Volumes.TotalBatchVolume / (Volumes.TotalBatchVolume + GrainfatherCalculator.GRAINFATHER_BOILER_TO_FERMENTOR_LOSS);
                        var pts = totalBatchPoints * ((double)grain.Amount / 100d) * boilerLossPercent;
                        grain.AmountGrams = GravityAlorithms.GetGrainWeight(pts, grain.FermentableAdjunct.Potential);
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
                    var mashGrainBillWeight = Grist.Where(x => x.Stage == FermentableStage.Mash).Sum(x => x.AmountGrams);

                    var swv = GrainfatherCalculator.CalcSpargeWaterVolume(mashGrainBillWeight,
                        (Volumes.PreBoilVolume),
                        topUpVolume);
                    if (swv < 0)
                        swv = 0;
                    SpargeWaterVolumeLabel.Content = String.Format("Sparge water volume [L]: {0:0.#}", swv);

                    var mwv = GrainfatherCalculator.CalcMashVolume(mashGrainBillWeight);
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
                double ecb = ColorAlgorithms.CalculateColor(Grist.ToList(), Volumes);
                string refString = ColorAlgorithms.GetReferenceBeer(ecb);
                ColorLabel.Content = String.Format("Color [ECB]: {0:F1} eqv. to {1}", ecb, refString);

                recalculateIbu();

            }
        }




        private void recalculateIbu()
        {
            IbuLabel.Content = string.Format("IBU: {0}", IbuAlgorithms.CalcIbu(BoilHops.Where(x => x.Stage == HopAdditionStage.Boil), OriginalGravity, Volumes.TotalBatchVolume));

            foreach (HopAddition h in BoilHops)
            {
                h.AmountGrams = Math.Round((Volumes.TotalBatchVolume * h.Amount));
            }

        }



        private void updateGuiTextboxes()
        {
            BatchSizeVolumeTextBox.Text = Volumes.FinalBatchVolume.ToString();
            ExpectedOriginalGravityTextBox.Text = OriginalGravity.ToString();
            BoilTimeTextBox.Text = BoilTime.ToString();
            TopUpMashWaterVolumeTextBox.Text = TopUpMashWater.ToString();
            PreBoilVolumeTextBox.Text = Volumes.PreBoilTapOff.ToString();
        }
        #endregion

        #region Event handlers

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


        private void BatchSizeVolumeTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            BatchSizeVolumeTextBox = (TextBox)(sender);
            double val = 0;

            if (BatchSizeVolumeTextBox.Text.Equals(String.Empty) || Volumes == null)
                return;

            if (Grist != null &&
                 Double.TryParse(BatchSizeVolumeTextBox.Text, out val))
            {
                if (val > (GrainfatherCalculator.GRAINFATHER_MAX_PREBOILVOLUME - GrainfatherCalculator.CalcBoilOffVolume(BoilTime)))
                {
                    MessageBox.Show("Batch size is to big. Please reduce it");
                    return;
                }
            }

            Volumes.FinalBatchVolume = val;
            recalculateGrainBill();
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


        private void AddOtherIngredientsButton_Click(object sender, RoutedEventArgs e)
        {
            var w = new AddOtherIngredients();
            w.ShowDialog();
            if (w.Result != null)
            {
                OtherIngredientsList.Add(w.Result);
            }
        }
        #endregion

        private void PreBoilVolumeTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            double volRemoved = 0;
            string errMsg = null;

            if (PreBoilVolumeTextBox.Text.Equals(String.Empty) || Volumes == null)
                return;

            if (!Double.TryParse(PreBoilVolumeTextBox.Text, out volRemoved))
            {
                errMsg = "Please provide a valid volume in float format";
            }
            else
            {
                double maxPreVol = GrainfatherCalculator.CalcPreBoilVolume(Volumes.TotalBatchVolume, BoilTime);
                if (volRemoved >= maxPreVol)
                {
                    errMsg = String.Format("The volume exceeds the pre-boil volume with a batch size of {0:F1} liters",
                        Volumes.TotalBatchVolume);
                }
            }

            if (errMsg != null)
                MessageBox.Show(errMsg);
            else
            {
                Volumes.PreBoilTapOff = volRemoved;
                recalculateGrainBill();
            }
        }
    }
}



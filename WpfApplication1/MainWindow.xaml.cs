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
using System.Reflection;
using WpfApplication1;
using System.Windows.Threading;
using System.Net;
using Grpcproto;
using Grpc.Core;
using Google.Protobuf.WellKnownTypes;
using System.Collections.Specialized;
using WpfApplication1.Domain;
using GoogleDriveStorage.DriveQuickstart;

namespace GFCalc
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<GristPart> Grist { set; get; }
        public ObservableCollection<HopAddition> BoilHops { set; get; }
        public ObservableCollection<Domain.MashProfileStep> MashProfileList { get; set; }
        public ObservableCollection<OtherIngredient> OtherIngredientsList { get; set; }


        public double PreBoilVolume { get; set; }
        public double OriginalGravity { get; set; }
        public int GrainBillSize { get; set; }
        public int BoilTime { set; get; }
        public double TopUpMashWater { set; get; }
        public double PreBoilRemovedVolume { get; private set; }
        public int EstAtten { get; private set; }

        private BrewVolumes Volumes;

        private FermentableRepository MaltRepo;
        private HopsRepository HopsRepo;
        private GrainfatherCalculator gfc;

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger
    (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private DispatcherTimer dispatcherTimer;

        public MainWindow()
        {
            InitializeComponent();

            var assembly = Assembly.GetExecutingAssembly();
            var path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var fullpath = String.Format("{0}\\{1}", path, assembly.GetName().Name);
            try {
                if (!Directory.Exists(fullpath))
                    Directory.CreateDirectory(fullpath);
            }
            catch (Exception e)
            {
                MessageBox.Show(String.Format("Unable to create {0}. Exception {1}", fullpath, e));

            }
            HopsRepo = new HopsRepository();
            MaltRepo = new FermentableRepository();

            Grist = new ObservableCollection<GristPart>();
            MaltsListView.ItemsSource = Grist;
            BoilHops = new ObservableCollection<HopAddition>();
            HopsListView.ItemsSource = BoilHops;

            MashProfileList = new ObservableCollection<Domain.MashProfileStep>();
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

            gfc = new GrainfatherCalculator();
            gfc.MashEfficiency = (double)WpfApplication1.Properties.Settings.Default["MashEfficiency"];
          EstAtten = 78;
            updateGuiText();

            GrainBrainMenuItem.IsEnabled = false;

            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 5);
            dispatcherTimer.Start();

            MashProfileList.CollectionChanged += this.OnCollectionChanged;

            BatchSizeVolumeTextBox.Text = Volumes.FinalBatchVolume.ToString();
            ExpectedOriginalGravityTextBox.Text = OriginalGravity.ToString();
            BoilTimeTextBox.Text = BoilTime.ToString();
            TopUpMashWaterVolumeTextBox.Text = TopUpMashWater.ToString();
            PreBoilVolumeTextBox.Text = Volumes.PreBoilTapOff.ToString();



        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (MashProfileList.Count == 0)
            {
                MashHeatOverTimeTextBox.IsEnabled = false;
                MashHeatOverTimeLabel.IsEnabled = false;
            }
            else
            {
                MashHeatOverTimeTextBox.IsEnabled = true;
                MashHeatOverTimeLabel.IsEnabled = true;
            }
        }

        #region Grainbrain handling
        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            bool grainBrainConnected = false;
            if (GrainbrainNetDiscovery.ScanGrainBrains())
                   grainBrainConnected = true;
           
            handleGrainBrainStatus(grainBrainConnected);

            
 
        }

        private void handleBrewProcess(GrainBrainStatus status)
        {


            if (status.State == BrewStep.StrikeWaterTempReached)
            {
                //dispatcherTimer.Tick -= brewProcessTimer_Tick;
                dispatcherTimer.IsEnabled = false;

                var swrw = new StrikeWaterReachedWindow();
                swrw.ShowDialog();

                IPAddress ipAddr;
                if (!GrainbrainNetDiscovery.GetGrainBrainAddress(out ipAddr))
                    return;

                string addr = String.Format("{0}:50051", ipAddr);
                Channel ch = new Channel(addr, ChannelCredentials.Insecure);

                McServer.McServerClient cl = new McServer.McServerClient(ch);

                GrainsAddedNotify req = new GrainsAddedNotify();
                Empty resp = cl.GrainsAdded(req);
                ch.ShutdownAsync().Wait();

                //dispatcherTimer.Tick += brewProcessTimer_Tick;
                dispatcherTimer.IsEnabled = true;


            }

            if (status.State == BrewStep.MashDoneStartSparge)
            {
                dispatcherTimer.IsEnabled = false;

                var sdw = new SpargeDoneWindow();
                sdw.ShowDialog();

                IPAddress ipAddr;
                if (!GrainbrainNetDiscovery.GetGrainBrainAddress(out ipAddr))
                    return;

                string addr = String.Format("{0}:50051", ipAddr);

                Channel ch = new Channel(addr, ChannelCredentials.Insecure);

                McServer.McServerClient cl = new McServer.McServerClient(ch);

                SpargeDoneNotify req = new SpargeDoneNotify();
                Empty resp = cl.SpargeDone(req);
                ch.ShutdownAsync().Wait();

                dispatcherTimer.IsEnabled = true;

            }

            if (status.State == BrewStep.BoilDone)
            {
                dispatcherTimer.IsEnabled = false;

                var wcsw = new WortChillerSanitizedWindow();
                wcsw.ShowDialog();

                IPAddress ipAddr;
                if (!GrainbrainNetDiscovery.GetGrainBrainAddress(out ipAddr))
                    return;

                string addr = String.Format("{0}:50051", ipAddr);

                Channel ch = new Channel(addr, ChannelCredentials.Insecure);

                McServer.McServerClient cl = new McServer.McServerClient(ch);

                WortChillerSanitizedDoneNotify req = new WortChillerSanitizedDoneNotify();
                Empty resp = cl.WortChillerSanitizedDone(req);
                ch.ShutdownAsync().Wait();

                dispatcherTimer.IsEnabled = true;
            }

        }
        private void handleGrainBrainStatus(bool isConnectToGrainBrain)
        {

            GrainBrainMenuItem.IsEnabled = isConnectToGrainBrain;

            if (!isConnectToGrainBrain)
            {
                lblGrainBrainState.Text = "State - Not connected";
                lblRemainingTime.Text = "-";
                return;
            }

            var s = GrainbrainNetDiscovery.GetGrainBrainStatus();
            if (s.State == BrewStep.Idle)
            {
                lblGrainBrainState.Text = String.Format("State - {0}", s.State);
                lblRemainingTime.Text = "-";
                return;
            }

            lblGrainBrainState.Text = String.Format("State - {0}", s.State);


            string time;
            if (s.RemainingMashStepList.Count() != 0)
            {
                var ms = s.RemainingMashStepList.First();
                time = String.Format("{0} minutes left at {1}", ms.StepTime, ms.Temperature);
            }
            else
                time = String.Format("{0} minutes of boiling left", s.RemainingBoilTime);
            lblRemainingTime.Text = time;

            this.progressBar.Value = s.Progress;

            handleBrewProcess(s);

        }
        #endregion

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
                hopsCompositionChanged(w.hop);
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
                    hopsCompositionChanged(h);
            }
        }

        private void hopsCompositionChanged(HopAddition addedHop)
        {
            if (addedHop != null)
            {
                var vol = Volumes.FinalBatchVolume;
                if (addedHop.Stage == HopAdditionStage.Boil)
                    vol = Volumes.PostBoilVolume;

                addedHop.AmountGrams = addedHop.Amount * vol;
                BoilHops.Add(addedHop);
            }
            recalculateIbu();
        }


        #endregion

        #region MashStepListView event handlers
        private void MashStepListView_KeyDown(object sender, KeyEventArgs e)
        {

            if (Key.Delete == e.Key)
            {
                foreach (Domain.MashProfileStep listViewItem in ((ListView)sender).SelectedItems)
                {
                    MashProfileList.Remove(listViewItem);
                    break;
                }
            }

        }

        private void MashStepListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (MashStepListView.SelectedIndex < 0 ||
                MashStepListView.SelectedIndex >= MashProfileList.Count)
            {
                return;
            }

            MashStepTempTextBox.Text = MashProfileList.ToArray()[MashStepListView.SelectedIndex].Temperature.ToString();
            MashStepTimeTextBox.Text = MashProfileList.ToArray()[MashStepListView.SelectedIndex].StepTime.ToString();

            if (MashStepListView.SelectedIndex == 0)
                MashHeatOverTimeTextBox.Text = "0";
            else
                MashHeatOverTimeTextBox.Text = MashProfileList.ToArray()[MashStepListView.SelectedIndex].HeatOverTime.ToString();

            MashProfileList.Remove((Domain.MashProfileStep)MashStepListView.SelectedItem);
            AddMashStepButton.Content = "Update";

        }
        private void AddMashStepButton_Click(object sender, RoutedEventArgs e)
        {
            double temp;
            if (!double.TryParse(MashStepTempTextBox.Text, out temp))
                return;

            int stepTime;
            if (!int.TryParse(MashStepTimeTextBox.Text, out stepTime))
                return;

            int heatOverTime = 0;
            if (MashProfileList.Count > 0)
            {
                if (!int.TryParse(MashHeatOverTimeTextBox.Text, out heatOverTime))
                    return;
            }

            var mps = new Domain.MashProfileStep();
            mps.Temperature = temp;
            mps.StepTime = stepTime;
            mps.HeatOverTime = heatOverTime;
            MashProfileList.Add(mps);
            var ol = MashProfileList.OrderBy(x => x.Temperature).ToList();
            MashProfileList.Clear();
            foreach (Domain.MashProfileStep ms in ol)
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

            r.BatchVolume = Volumes.FinalBatchVolume;
            r.PreBoilTapOffVolume = Volumes.PreBoilTapOff;
            r.TopUpMashWater = 0;

            r.OriginalGravity = OriginalGravity;
            r.BoilTime = BoilTime;


            // Create OpenFileDialog 
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();



            // Set filter for file extension and default file extension 
            dlg.DefaultExt = ".gsrx";
            dlg.Filter = "Grainfather recipe files (*.gsrx)|*.gsrx";
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
            GoogleDriveStorager gd = new GoogleDriveStorager();
            gd.TestGd();

            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();



            // Set filter for file extension and default file extension 
            dlg.Filter = "Grainfather recipe files (*.gsrx)|*.gsrx|Beersmith2 file (*.bsmx)|*.bsmx";
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

            try
            {
                Recepie loadedObject = (Recepie)serializer.Deserialize(reader);
                PopulateGUI(loadedObject);

            }
            catch (Exception e)
            {
                MessageBox.Show("The recipe can be parsed. {0}", e.ToString());
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
            foreach (Domain.MashProfileStep mps in aRecepie.MashProfile)
                MashProfileList.Add(mps);

            OtherIngredientsList.Clear();
            foreach (OtherIngredient o in aRecepie.OtherIngredients)
                OtherIngredientsList.Add(o);


            Volumes.FinalBatchVolume = aRecepie.BatchVolume;
            BatchSizeVolumeTextBox.Text = Volumes.FinalBatchVolume.ToString();

            Volumes.PreBoilTapOff = aRecepie.PreBoilTapOffVolume;
            PreBoilVolumeTextBox.Text = Volumes.PreBoilTapOff.ToString();

            TopUpMashWater = aRecepie.TopUpMashWater;
            TopUpMashWaterVolumeTextBox.Text = TopUpMashWater.ToString();

            OriginalGravity = aRecepie.OriginalGravity;
            ExpectedOriginalGravityTextBox.Text = OriginalGravity.ToString();

            BoilTime = aRecepie.BoilTime;
            BoilTimeTextBox.Text = BoilTime.ToString();

            NameTextBox.Text = aRecepie.Name;


            recalculateGrainBill();

            recalculateIbu();

            updateGuiText();
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
                doc.PageHeight = pDialog.PrintableAreaHeight;
                doc.PageWidth = pDialog.PrintableAreaWidth;
                doc.ColumnWidth = doc.PageWidth;

                doc.FontFamily = new FontFamily("Courier");
                doc.FontSize = 11;



                #region Print mash part
                StringBuilder strbcs = new StringBuilder();
                StringBuilder strbmash = new StringBuilder();
                StringBuilder strbferm = new StringBuilder();
                foreach (GristPart g in Grist)
                {

                    switch (g.Stage)
                    {
                        case FermentableStage.ColdSteep:
                            strbcs.Append("Add " + g.ToString() + "\n");
                            break;
                        case FermentableStage.Mash:
                            strbmash.Append("Add " + g.ToString() + "\n");
                            break;
                        case FermentableStage.Fermentor:
                            strbferm.Append("Add " + g.ToString() + "\n");
                            break;
                        default:
                            break;
                    }
                }

                var recepieHeading = new Paragraph(new Bold(new Run(String.Format("Recipe for {0}, approx. {1:F1} L in fermentor", NameTextBox.Text, Volumes.FinalBatchVolume))));

                var mashHeading = new Paragraph(new Bold(new Run("Mash")));
                mashHeading.FontSize = recepieHeading.FontSize = 18;
                doc.Blocks.Add(recepieHeading);
                doc.Blocks.Add(mashHeading);

                if (Grist.Any(x => x.Stage == FermentableStage.ColdSteep))
                {
                    Paragraph pcs = new Paragraph();
                    pcs.Inlines.Add(new Bold(new Run("Cold steep, 24 before brew start:\n")));
                    pcs.Inlines.Add(new Run(strbcs.ToString()));
                    doc.Blocks.Add(pcs);
                }
                Paragraph pcsw = new Paragraph();
                pcsw.Inlines.Add(new Run(String.Format("Mix with {0:F1} cold water\n", (Volumes.ColdSteepVolume * (1 - ColdSteep.COLDSTEEP_VOLUME_TO_SPARGE_RATIO)))));


                Paragraph pm = new Paragraph();
                pm.Inlines.Add(new Bold(new Run("Normal step mash:\n")));

                pm.Inlines.Add(new Run(String.Format("Add {0:F1} liters of water to Grainfather for mashing\n\n",
                    GrainfatherCalculator.CalcMashVolume(GrainBillSize))));
                pm.Inlines.Add(new Run(strbmash.ToString()));
                doc.Blocks.Add(pm);


                StringBuilder strmp = new StringBuilder("");
                foreach (Domain.MashProfileStep mss in MashProfileList)
                {
                    strmp.Append("Mash at " + mss.ToString() + "\n");
                }


                Paragraph pmp = new Paragraph();
                pmp.Inlines.Add(new Bold(new Run("Mash profile:\n")));
                pmp.Inlines.Add(new Run(strmp.ToString()));

                pmp.Inlines.Add(new Run(String.Format("\nSparge with {0:F1} liters of 78 C water\n",
                    GrainfatherCalculator.CalcSpargeWaterVolume(GrainBillSize,
                    (Volumes.PreBoilVolume),
                    TopUpMashWater))));


                var prbg = GravityAlgorithms.GetGravity(
                    Grist.Where(x => x.Stage == FermentableStage.Mash).Sum(x => x.GU), Volumes.PreBoilVolume);


                var pobg = GravityAlgorithms.GetGravity(Grist.Where(x => x.Stage != FermentableStage.Fermentor).Sum(x => x.GU),
                    Volumes.PostBoilVolume);


                pmp.Inlines.Add(new Run(String.Format("\nExpected post-mash gravity is {0:F3}. Post-mash volume shall be {1:F1} liters",
                    prbg, Volumes.PreBoilVolume + Volumes.PreBoilTapOff)));
                if(Volumes.PreBoilTapOff > 0)
                {
                    pmp.Inlines.Add(new Run(String.Format("\nTap off {0:F1} liters wort before boil start. Expected Pre-boil volume is {1:F1} liters.\n", 
                        Volumes.PreBoilTapOff,
                        Volumes.PreBoilVolume)));
                }

                doc.Blocks.Add(pmp);

                #endregion


                #region Boiling part
                var boilingHeader = (new Paragraph(new Bold(new Run("Boiling"))));
                boilingHeader.FontSize = 16;
                doc.Blocks.Add(boilingHeader);

                StringBuilder strbboil = new StringBuilder("");
                StringBuilder str_ferms_boil = new StringBuilder("");

                foreach (GristPart g in Grist)
                {
                    if (g.Stage == FermentableStage.Boil)
                    {
                        str_ferms_boil.Append("Add " + g.ToString() + "\n");
                    }
                }



                    
                StringBuilder strbdry = new StringBuilder("");
                foreach (HopAddition g in BoilHops)
                {
                    switch (g.Stage)
                    {
                        case HopAdditionStage.Boil:
                            strbboil.Append("Add " + g.ToString() + "\n");
                            break;
                        case HopAdditionStage.Fermentation:
                            strbboil.Append("Add " + g.ToString() + "\n");
                            break;
                        default:
                            break;
                    }
                }
                Paragraph pbh = new Paragraph();
                pbh.Inlines.Add(new Bold(new Run("Hop additions:\n")));
                pbh.Inlines.Add(new Run(strbboil.ToString()));



                var vm = Volumes.PreBoilVolume;
                var vcs = Volumes.ColdSteepVolume;

                foreach (GristPart g in Grist)
                {
                    if (g.Stage == FermentableStage.ColdSteep)
                        pbh.Inlines.Add(new Run(String.Format("Add the runnings of {0} to the boil 10 minutes before end\n", g.FermentableAdjunct.Name)));
                }

                if (Grist.Any(x => x.Stage == FermentableStage.ColdSteep))
                {
                    pbh.Inlines.Add(new Run(String.Format("\nSparge all runnings with {0:F1}L water.\n", Volumes.ColdSteepVolume * (ColdSteep.COLDSTEEP_VOLUME_TO_SPARGE_RATIO))));
                }


                pbh.Inlines.Add(new Run(String.Format("Expected post-boil gravity is {0:F3}. Post-boil volume shall be {1:F1} liters", pobg, Volumes.PostBoilVolume)));

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

                // Create IDocumentPaginatorSource from FlowDocument
                IDocumentPaginatorSource idpSource = doc;

                // Call PrintDocument method to send document to printer
                pDialog.PrintDocument(idpSource.DocumentPaginator, NameTextBox.Text);
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
                var preBoilTappOffLoss = (Volumes.PreBoilTapOff / (Volumes.PreBoilVolume + Volumes.PreBoilTapOff));

                var totalGravity = GravityAlgorithms.GetTotalGravity(Volumes.PostBoilVolume, OriginalGravity);


                Volumes.ColdSteepVolume = 0;

                var l = new List<GristPart>();
                foreach (var grain in Grist)
                {
                    grain.GU = totalGravity * ((double)grain.Amount / 100d);

                    if (grain.Stage == FermentableStage.ColdSteep)
                    {

                        grain.AmountGrams = GravityAlgorithms.GetGrainWeight(grain.GU, grain.FermentableAdjunct.Potential, ColdSteep.COLDSTEEP_EFFICIENCY);

                        Volumes.ColdSteepVolume += ColdSteep.GetColdSteepWaterContibution(grain.AmountGrams);
                    }

                    if (grain.Stage == FermentableStage.Mash)
                    {
                        grain.AmountGrams = GravityAlgorithms.GetGrainWeight(grain.GU, grain.FermentableAdjunct.Potential, gfc.MashEfficiency);

                        grain.AmountGrams += (int)Math.Round(grain.AmountGrams * preBoilTappOffLoss);
                    }

                    if (grain.Stage == FermentableStage.Fermentor)
                    {
                        grain.AmountGrams = GravityAlgorithms.GetGrainWeight(grain.GU, grain.FermentableAdjunct.Potential, 1);

                        var boilerLossPercent =  GrainfatherCalculator.GRAINFATHER_BOILER_TO_FERMENTOR_LOSS / (Volumes.PostBoilVolume);

                        grain.AmountGrams -= (int)Math.Round(boilerLossPercent * grain.AmountGrams);
                    }

                    l.Add(grain);

                }

                GrainBillSize = Grist.Where(x => x.Stage == FermentableStage.Mash).Sum(x => x.AmountGrams);

                foreach (var grain in l)
                {
                    Grist.Remove(grain);
                    Grist.Add(grain);
                }

                if (GrainBillSize > GrainfatherCalculator.SMALL_GRAINBILL || GrainBillSize == 0)
                {
                    TopUpMashWaterVolumeTextBox.Visibility = Visibility.Hidden;
                    TopUpMashWaterVolumeLabel.Visibility = Visibility.Hidden;

                }
                else
                {
                    TopUpMashWaterVolumeTextBox.Visibility = Visibility.Visible;
                    TopUpMashWaterVolumeLabel.Visibility = Visibility.Visible;
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

                var ol = BoilHops.OrderBy(x => x).ToList();
                BoilHops.Clear();
                foreach (HopAddition h in ol)
                    hopsCompositionChanged(h);
                
            }
            updateGuiText();
        }




        private void recalculateIbu()
        {
            var Gravity = GravityAlgorithms.GetGravity(Volumes.PreBoilVolume, Grist.Where(x => x.Stage == FermentableStage.Mash).ToList(), GrainfatherCalculator.mashEfficiency);
            var ibu = IbuAlgorithms.CalcIbu(BoilHops.Where(x => x.Stage == HopAdditionStage.Boil), Gravity, Volumes.PostBoilVolume);
            var highIbu = ibu + IbuAlgorithms.IBU_TOLERANCE * ibu;
            var lowIbu = ibu - IbuAlgorithms.IBU_TOLERANCE * ibu;

            IbuLabel.Content = string.Format("IBU: {0:F0} - {1:F0} ({2:F0})", lowIbu, highIbu, ibu);

        }


        private void updateGuiText()
        {


            EstAttenTextBox.Text = EstAtten.ToString();

            MashVolumeLabel.Content = new StringBuilder()
                .AppendFormat("Mash water volume: {0:F1} liters", GrainfatherCalculator.CalcMashVolume(GrainBillSize)).ToString();

            SpargeVolumeLabel.Content = new StringBuilder()
                .AppendFormat("Sparge water volume: {0:F1} liters", 
                GrainfatherCalculator.CalcSpargeWaterVolume(GrainBillSize, (Volumes.PreBoilVolume + Volumes.PreBoilTapOff), TopUpMashWater))
                .ToString();

            PreBoilDataLabel.Content = new StringBuilder()
                .AppendFormat("Expected pre-boil gravity is {0:F3}, preboil volume {1:F1} liters", GravityAlgorithms.GetGravity(
                    Grist.Where(x => (x.Stage != FermentableStage.Fermentor && x.Stage != FermentableStage.ColdSteep)).Sum(x => x.GU), Volumes.PreBoilVolume), Volumes.PreBoilVolume).ToString();


            PostBoilDataLabel.Content = new StringBuilder()
                .AppendFormat("Expected post-boil gravity is {0:F3}, postboil volume {1:F1} liters", OriginalGravity, Volumes.PostBoilVolume).ToString();

            AbvDataLabel.Content = new StringBuilder().AppendFormat("Expected ABV is {0:F2} %", 
                Abv.CalculateAbv(OriginalGravity, ((OriginalGravity - 1) * (1 - ((double)EstAtten / 100))) + 1)).ToString();
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

            if (BoilTimeTextBox.Text.Equals(String.Empty))
                return;

            if (Grist != null &&
                 Int32.TryParse(BoilTimeTextBox.Text, out val))
            {
                BoilTime = val;
               Volumes.BoilOffLoss = GrainfatherCalculator.CalcBoilOffVolume(BoilTime);
                recalculateGrainBill();
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

        private void EstAttenTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (EstAttenTextBox.Text.Equals(String.Empty) || Volumes == null)
                return;

            int ea;
            string errMsg = null;
            if (!int.TryParse(EstAttenTextBox.Text, out ea))
            {
                errMsg = "Please provide a valid estimated attenuation in integer format";
            }
            else
            {
                EstAtten = ea;
            }

            if (errMsg != null)
                MessageBox.Show(errMsg);
            else
            {
                recalculateGrainBill();
            }
        }


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
                if (volRemoved >= Volumes.PreBoilVolume)
                {
                    errMsg = String.Format("The volume exceeds the pre-boil volume with a batch size of {0:F1} liters",
                        Volumes.PostBoilVolume);
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

        #endregion

        #region Misc menu items
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            var aboutWindow = new WpfApplication1.About();
            aboutWindow.ShowDialog();
        }

        private void MenuItem_SettingsMashEfficiency(object sender, RoutedEventArgs e)
        {
            var meWindow = new MashEffeciency((int)(gfc.MashEfficiency*100d));
            meWindow.ShowDialog();

            double me = 0;
            if (meWindow.Gravity != 0 && meWindow.Volume != 0)
            {

                var measuredPts = GravityAlgorithms.GetPoints(meWindow.Gravity, meWindow.Volume);
                var possiblePts = Grist.Where(x => x.Stage == FermentableStage.Mash).
                    Sum( v => v.GU);

                me = measuredPts / possiblePts;
                MessageBox.Show(String.Format("Mash efficiency updated to {0:F0}%, will update settings", me * 100));
            }
            else if (meWindow.MashEfficiency != 0)
                me = (double)meWindow.MashEfficiency/100d;
            else
                return;
            

            gfc.MashEfficiency = me;
            WpfApplication1.Properties.Settings.Default["MashEfficiency"] = me;
            WpfApplication1.Properties.Settings.Default.Save();

            recalculateGrainBill();
            recalculateIbu();

        }
        #endregion

        #region Grainbrain menu items
        private void MenuItem_GrainbrainStart(object sender, RoutedEventArgs e)
        {

            var sgw = new StartGrainbrainWindow();
            sgw.ShowDialog();

            LoadBrewProfileRequest req = new LoadBrewProfileRequest();
            req.BoilTime = BoilTime;

            foreach (Domain.MashProfileStep ms in  MashProfileList)
            {
                req.MashProfileSteps.Add(new Grpcproto.MashProfileStep { Temperature = (int)(Math.Round(ms.Temperature)), StepTime = ms.StepTime, HeatOverTime = ms.HeatOverTime });
                log.Info(String.Format("Mash time {0} at {1} C", ms.StepTime, ms.Temperature));
            }

            foreach (HopAddition bh in BoilHops)
            {
                if (bh.Stage == HopAdditionStage.Boil)
                {
                    req.HopAdditionStep.Add(new HopAdditionStep { Time = bh.Duration, Name = bh.Hop.Name });
                    log.Info(String.Format("{0} at {1} min", bh.Hop.Name, bh.Duration));
                }

            }

            req.GrainbillWeight = GrainBillSize;
            req.MashWaterVolume = Volumes.FinalBatchVolume;
            IPAddress ipAddr;
            GrainbrainNetDiscovery.GetGrainBrainAddress(out ipAddr);
            string addr = String.Format("{0}:50051", ipAddr);
            Channel channel = new Channel(addr, ChannelCredentials.Insecure);

            McServer.McServerClient client = new McServer.McServerClient(channel);
            SuccessReply reply = client.LoadBrewProfile(req);
            
            StartStopRequest startReq = new StartStopRequest();
            startReq.StartStop = StartStopRequest.Types.StartStop.Start;
            SuccessReply relpy2 = client.StartStopAbort(startReq);
            log.Info("Started - Response: " + reply.Success.ToString());


        }
        
        private void MenuItem_GrainbrainBrewingMonitor(object sender, RoutedEventArgs e)
        {
            IPAddress ipAddr;
            if (GrainbrainNetDiscovery.GetGrainBrainAddress(out ipAddr))
            {
                var bm = new BrewingMonitor(ipAddr);
                bm.ShowDialog();
            }
        }

        private void MenuItem_GrainbrainStop(object sender, RoutedEventArgs e)
        {
            var sbpw = new StopBrewingProcessWindow();
            sbpw.ShowDialog();
            if (sbpw.Abort)
            {
                IPAddress ipAddr;
                if (!GrainbrainNetDiscovery.GetGrainBrainAddress(out ipAddr))
                    return;
                string addr = String.Format("{0}:50051", ipAddr);

                Channel ch = new Channel(addr, ChannelCredentials.Insecure);

                McServer.McServerClient cl = new McServer.McServerClient(ch);

                StartStopRequest req = new StartStopRequest();
                req.StartStop = StartStopRequest.Types.StartStop.Stop;
                SuccessReply resp = cl.StartStopAbort(req);
                ch.ShutdownAsync().Wait();

            }
        }
        #endregion

      
    }
}



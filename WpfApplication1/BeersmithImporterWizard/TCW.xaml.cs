﻿using BSImport;
using GFCalc.DataModel;
using GFCalc.Domain;
using GFCalc.Repos;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Grainsim.Domain;
using System.Diagnostics;

namespace Grainsim.BeersmithImporterWizard
{
    /// <summary>
    /// Interaction logic for TCW.xaml
    /// </summary>
    public partial class TCW : Window
    {
        public ObservableCollection<FermentableAdjunct> FermentablesObservableList { set; get; }
        public ObservableCollection<Hops> HopsObservableList { get; set; }
        public List<BSGrainBill> BSGrainBill { get; private set; }
        public List<BSHops> BSBoilHops { get; private set; }

        private HopsRepository HopsRepo;
        private FermentableRepository MaltsRepo;

        private BSImporter BeersmithImporter;

        private Recepie WorkRecepie;
        public Recepie ImportedRecepie { get; set; }

        public TCW(string aBSExportFilename, FermentableRepository aMaltRepo, HopsRepository aHopsRepo)
        {
            InitializeComponent();
            this.MaltsRepo = aMaltRepo;
            this.HopsRepo = aHopsRepo;

            FermentablesObservableList = new ObservableCollection<FermentableAdjunct>(aMaltRepo.Get());
            HopsObservableList = new ObservableCollection<Hops>(aHopsRepo.Get());

            BeersmithImporter = new BSImporter(aBSExportFilename);
            RecipeNameCombobox.ItemsSource = BeersmithImporter.GetAllRecipes();
            RecipeNameCombobox.SelectedIndex = 0;

            HopsListView.ItemsSource = HopsObservableList;
            MaltsListView.ItemsSource = FermentablesObservableList;

            WorkRecepie = new Recepie();

        }

        private void ChangeTabItem(int aChange)
        {
            int newTabIndex = tabControl.SelectedIndex + aChange;
            if (newTabIndex >= tabControl.Items.Count)
            {
                newTabIndex = 0;
            }
            else if (newTabIndex < 0)
            {
                newTabIndex = tabControl.Items.Count - 1;
            }

            tabControl.SelectedIndex = newTabIndex;
        }

        private void handleSelectHops()
        {
            HopAddition h = new HopAddition();
            if (HopsListView.SelectedItem != null)
            {
                var bsh = BSBoilHops.First();

                h.Hop = (Hops)(HopsListView.SelectedItem);
                Debug.Assert(WorkRecepie.BatchVolume != 0);
                h.Amount = (bsh.Amount) / (WorkRecepie.BatchVolume);
                h.Duration = bsh.BoilTime;


                WorkRecepie.BoilHops.Add(h);
                var del = BSBoilHops.First();
                BSBoilHops.Remove(del);


                if (BSGrainBill.Count == 0)
                {
                    ImportedRecepie = WorkRecepie;
                    this.Close();
                }
                else
                    TextblockHops.Text = "Please select a corresponding hops for " + BSBoilHops.First().Name + " with alpha acid " + BSBoilHops.First().AlphaAcid.ToString();

            }
            else
                MessageBox.Show("Please select a hop in the list to match the imported one");
        }

        private void handleSelectMalts()
        {
            GristPart m = new GristPart();
            if (MaltsListView.SelectedItem != null)
            {
                var bsfm = BSGrainBill.First();
                m.FermentableAdjunct = (FermentableAdjunct)(MaltsListView.SelectedItem);
                m.Amount = bsfm.AmountPercent;
                m.Stage = FermentableStage.Mash;

                WorkRecepie.Fermentables.Add(m);
                var del = BSGrainBill.First();
                BSGrainBill.Remove(del);


                if (BSGrainBill.Count == 0)
                {
                    BSBoilHops = BeersmithImporter.GetBoilHops(WorkRecepie.Name).ToList();
                    ChangeTabItem(1);
                    TextblockHops.Text = "Please select a corresponding hops for " + BSBoilHops.First().Name + " with alpha acid " + BSBoilHops.First().AlphaAcid.ToString();
                }
                else
                    TextblockMalts.Text = "Please select a corresponding malt for " + BSGrainBill.First().FermentableName + ". " + BSGrainBill.First().AmountPercent.ToString() + " % of total grist";

            }
            else
                MessageBox.Show("Please select a fermentable adjunct in the list");

        }

        private void handleSelectRecipeNext()
        {
            var r = RecipeNameCombobox.Text;
            if (r != null && !r.Equals(String.Empty))
            {
                WorkRecepie.Name = r;
                WorkRecepie.BatchVolume = BeersmithImporter.getFinalBatchVolume(WorkRecepie.Name);
                WorkRecepie.BoilTime = BeersmithImporter.getBoilTime(WorkRecepie.Name);
                WorkRecepie.OriginalGravity = BeersmithImporter.getOriginalGravity(WorkRecepie.Name);


                BSGrainBill = BeersmithImporter.GetGrainBill(WorkRecepie.Name).ToList();

                // Next step
                ChangeTabItem(1);

                TextblockMalts.Text = "Please select a corresponding malt for " + BSGrainBill.First().FermentableName + ". " + BSGrainBill.First().AmountPercent.ToString() + " % of total grist";
            }
            else
                MessageBox.Show("Please select a recepie");
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            switch (tabControl.SelectedIndex)
            {
                case 0:
                    handleSelectRecipeNext();
                    break;
                case 1:
                    handleSelectMalts();
                    break;
                case 2:
                    handleSelectHops();
                    break;
                default:
                    break;

            }

        }

        private void FinishButton_Click(object sender, RoutedEventArgs e)
        {
             this.Close();
        }
    }
}

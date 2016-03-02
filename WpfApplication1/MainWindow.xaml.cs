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
using GFCalc.DataModel;
using GFCalc.Domain;
using GFCalc.Repos;
using WpfApplication1;
using WpfApplication1.Domain;

namespace GFCalc
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<GristPart> grist { set; get; }
        public ObservableCollection<HopBoilAddition> BoilHops { set; get; }

        public double BatchSize { get; set; }
        public double PreBoilVolume { get; set; }
        public double OriginalGravity { get; set; }
        public double PreBoilGravity { get; set; }
        public double MashEfficieny { get; set; }
        public double GrainBillSize { get; set; }



        public MainWindow()
        {
            InitializeComponent();

            grist = new ObservableCollection<GristPart>();
            MaltsListView.ItemsSource = grist;
            BoilHops = new ObservableCollection<HopBoilAddition>();
            HopsListView.ItemsSource = BoilHops;

            BatchSize = 25;
            BatchSizeVolumeTextBox.Text = BatchSize.ToString();

            OriginalGravity = 1.05;
            ExpectedOriginalGravityTextBox.Text = OriginalGravity.ToString();

        }

        private void addGrains_Click(object sender, RoutedEventArgs e)
        {
            var sum = grist.Sum(g => g.Amount);
            var sw = new SelectGrain(sum);
            sw.ShowDialog();
            grist.Add(sw.Result);

            recalculate();
        }

        private void listView_KeyDown(object sender, KeyEventArgs e)
        {
            if (Key.Delete == e.Key)
            {

                foreach (GristPart listViewItem in ((ListView)sender).SelectedItems)
                {
                    grist.Remove(listViewItem);
                    MessageBox.Show(String.Format("Shit, want to remove {0}. That is {1} away", listViewItem.FermentableAdjunct.Name, listViewItem.Amount));
                    break;
                }
            }
        }

        private void recalculate()
        {
            var sum = grist.Sum(g => g.Amount);
            if (sum != 100)
            {
                sum = -1;
                GrainBillSize = 0;
            }
            else
                GrainBillSize = GravityAlorithms.GetMashGrainBillWeight(OriginalGravity, BatchSize, grist.ToList(), null, GrainFatherCalculator.MashEfficiency);
                
            var l = new List<GristPart>();
            foreach (var grain in grist)
            {
                grain.AmountKg = (grain.Amount * GrainBillSize) / sum;
                l.Add(grain);
            }

            foreach (var grain in l)
            {
                grist.Remove(grain);
                grist.Add(grain);
            }
        }


    private void ExpectedOriginalGravityTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        ExpectedOriginalGravityTextBox = (TextBox)(sender);
        double val = 0;

        if (grist != null &&
            Double.TryParse(ExpectedOriginalGravityTextBox.Text, out val))
        {
            OriginalGravity = val;
            recalculate();
        }
    }

    private void button_Click(object sender, RoutedEventArgs e)
    {
        var w = new SelectHops();
        w.ShowDialog();
        BoilHops.Add(w.hop);
        IbuLabel.Content = string.Format("IBU: {0}", IbuAlgorithms.CalcIbu(BoilHops.ToList<HopBoilAddition>(), 1.047, 25));
    }

    private void MaltsListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {

        if (MaltsListView.SelectedIndex >= grist.Count() || MaltsListView.SelectedIndex < 0)
            return;

        double sum = grist.Sum(g => g.Amount);
        sum -= grist.ToArray()[MaltsListView.SelectedIndex].Amount;
        var sw = new SelectGrain(sum, grist.ToArray()[MaltsListView.SelectedIndex]);
        sw.ShowDialog();
        grist.Remove((GristPart)MaltsListView.SelectedItem);
        grist.Add(sw.Result);

        recalculate();
    }

    private void BatchSizeVolumeTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        BatchSizeVolumeTextBox = (TextBox)(sender);
        double val = 0;
        if (grist != null &&
             Double.TryParse(BatchSizeVolumeTextBox.Text, out val))
        {
            BatchSize = val;
            recalculate();
        }
    }


    private void HopsListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        var w = new SelectHops(BoilHops.ToArray()[HopsListView.SelectedIndex]);
        w.ShowDialog();
        BoilHops.Remove((HopBoilAddition)HopsListView.SelectedItem);
        BoilHops.Add(w.hop);

        recalculate();
    }


}

}

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
        public ObservableCollection<HopAddition> BoilHops { set; get; }

        public MainWindow()
        {
            InitializeComponent();

            grist = new ObservableCollection<GristPart>();
            listView.ItemsSource = grist;
            BoilHops = new ObservableCollection<HopAddition>();
            listView1.ItemsSource = BoilHops;
        }

        private void addGrains_Click(object sender, RoutedEventArgs e)
        {
            var sw = new SelectGrain();
            sw.ShowDialog();
            AddToGrist(sw.Result);
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

        //public void grainBillTextBox_DragLeave

        public void AddToGrist(GristPart aGristPart)
        {
            grist.Add(aGristPart);
        }


        private void grainBillTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            grainBillTextBox = (TextBox)(sender);
            Double size;
            if (Double.TryParse(grainBillTextBox.Text, out size))
            {
                var vol = GrainFatherCalculator.CalculateMashVolume(size);
                if (labelTotalMashVolume != null)
                    labelTotalMashVolume.Content = string.Format("Total mash volume [L]: {0} ", vol);
            }
        }

        private void OGTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            OGTextBox = (TextBox)(sender);
            Double size;
            if (Double.TryParse(OGTextBox.Text, out size))
            {
                var vol = GrainFatherCalculator.CalculateMashVolume(size);
                if (labelTotalMashVolume != null)
                    labelTotalMashVolume.Content = string.Format("Total mash volume [L]: {0} ", vol);
            }
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            var w = new SelectHops();
            w.ShowDialog();
            BoilHops.Add(w.hop);
        }
    }

}

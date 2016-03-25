using GFCalc.DataModel;
using GFCalc.Repos;
using System;
using System.Collections.Generic;
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

namespace Grainsim
{
    /// <summary>
    /// Interaction logic for SelectHops.xaml
    /// </summary>
    public partial class SelectHops : Window
    {
        public HopBoilAddition hop { set; get; }

        public SelectHops(HopsRepository aRepo, int aBoilTime)
        {
            InitializeComponent();

            var hopsRepo = aRepo;
            var hops = hopsRepo.Get();
            HopsComboBox.ItemsSource = hops;
            TimeDurationTextBox.Text = aBoilTime.ToString();
        }

        public SelectHops(HopsRepository aRepo, HopBoilAddition aHopAddition)
        {
            InitializeComponent();

            var hopsRepo = aRepo;
            var hops = hopsRepo.Get();
            HopsComboBox.ItemsSource = hops;

            HopsComboBox.SelectedValue = hops.FirstOrDefault(x => x.Name.Equals(aHopAddition.Hop.Name));

            TimeDurationTextBox.Text = aHopAddition.Minutes.ToString();
            AmountTextBox.Text = aHopAddition.Amount.ToString();

        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            Hops var = (Hops)HopsComboBox.SelectedItem;
            hop = new HopBoilAddition();

            if (var == null)
            {
                MessageBox.Show("Please select a hop in the dowp down menu");
                return;
            }
            hop.Hop = var;

            string stage = StageComboBox.Text;
            if (stage == null)
            {
                MessageBox.Show("Please select a hop in the dowp down menu");
                return;
            }
            hop.Stage = stage;

            int timeMinutes;
            if (int.TryParse(TimeDurationTextBox.Text, out timeMinutes))
            {
                hop.Minutes = timeMinutes;
                this.Close();
            }
            else
                MessageBox.Show(String.Format("Unable to interperate {0} as a integer value. Please proviode a correct integer value value in Time", TimeDurationTextBox.Text));


            float amount;
            if (float.TryParse(AmountTextBox.Text, out amount))
            {
                hop.Amount = amount;
                this.Close();
            }
            else
                MessageBox.Show(String.Format("Unable to interperate {0} as a float value. Please proviode a correct float value in Part", AmountTextBox.Text));


        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}

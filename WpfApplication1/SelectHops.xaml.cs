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
        public HopAddition hop { set; get; }

        public SelectHops(HopsRepository aRepo, int aBoilTime)
        {
            InitializeComponent();

            var hopsRepo = aRepo;
            var hops = hopsRepo.Get();
            HopsComboBox.ItemsSource = hops;
            HopsComboBox.SelectedIndex = 0;
            TimeDurationTextBox.Text = aBoilTime.ToString();
            StageComboBox.ItemsSource = Enum.GetValues(typeof(HopAdditionStage)).Cast<HopAdditionStage>();
            StageComboBox.SelectedIndex = 0;
        }

        public SelectHops(HopsRepository aRepo, HopAddition aHopAddition)
        {
            InitializeComponent();

            var hopsRepo = aRepo;
            var hops = hopsRepo.Get();
            HopsComboBox.ItemsSource = hops;

            HopsComboBox.SelectedValue = hops.FirstOrDefault(x => x.Name.Equals(aHopAddition.Hop.Name));

            TimeDurationTextBox.Text = aHopAddition.Duration.ToString();
            AmountTextBox.Text = aHopAddition.Amount.ToString();
            StageComboBox.ItemsSource = Enum.GetValues(typeof(HopAdditionStage)).Cast<HopAdditionStage>();
            StageComboBox.SelectedItem = (HopAdditionStage)aHopAddition.Stage; 

        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            Hops var = (Hops)HopsComboBox.SelectedItem;
            hop = new HopAddition();

            if (var == null)
            {
                MessageBox.Show("Please select a hop in the drop down menu");
                return;
            }
            hop.Hop = var;

            var stage = (HopAdditionStage)StageComboBox.SelectedItem;
            if (StageComboBox.SelectedIndex == 0)
            {
                MessageBox.Show("Please select a hop in the drop down menu");
                return;
            }
            hop.Stage = stage;

            int timeMinutes;
            if (int.TryParse(TimeDurationTextBox.Text, out timeMinutes))
            {
                hop.Duration = timeMinutes;
                this.Close();
            }
            else
                MessageBox.Show(String.Format("Unable to interpreter {0} as a integer value. Please provide a correct integer value in Time", TimeDurationTextBox.Text));


            float amount;
            if (float.TryParse(AmountTextBox.Text, out amount))
            {
                hop.Amount = Math.Round(amount, 2);
                this.Close();
            }
            else
                MessageBox.Show(String.Format("Unable to interpreter {0} as a float value. Please provide a correct float value in Part", AmountTextBox.Text));


        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void StageComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((HopAdditionStage)(StageComboBox.SelectedItem) == (HopAdditionStage.Fermentation))
            {
                TimeDuration.Content = "Time[Days]:";
            }
            else
            {
                TimeDuration.Content = "Time[minutes from boil end]:";
            }
        }
    }
}

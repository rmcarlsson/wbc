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

            UnitCheckBox.IsChecked = false;
        }

        public SelectHops(HopsRepository aRepo, HopAddition aHopAddition)
        {
            InitializeComponent();

            var hopsRepo = aRepo;
            var hops = hopsRepo.Get();
            HopsComboBox.ItemsSource = hops;

            HopsComboBox.SelectedValue = hops.FirstOrDefault(x => x.Name.Equals(aHopAddition.Hop.Name));

            TimeDurationTextBox.Text = aHopAddition.Duration.ToString();
            StageComboBox.ItemsSource = Enum.GetValues(typeof(HopAdditionStage)).Cast<HopAdditionStage>();
            StageComboBox.SelectedItem = (HopAdditionStage)aHopAddition.Stage;

            if (aHopAddition.AmountUnit == HopAmountUnitE.IBU)
            {
                UnitCheckBox.IsChecked = false;
                AmountLabel.Content = "Bitterness [IBU] :";
                AmountTextBox.Text = aHopAddition.Bitterness.ToString();
            }
            else
            {
                UnitCheckBox.IsChecked = true;
                AmountLabel.Content = "Amount [g/L] :";
                AmountTextBox.Text = aHopAddition.Amount.ToString();
            }
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
            hop.Stage = stage;

            int timeMinutes;
            if (int.TryParse(TimeDurationTextBox.Text, out timeMinutes))
            {
                hop.Duration = timeMinutes;
            }
            else
                MessageBox.Show(String.Format("Unable to interpreter {0} as a integer value. Please provide a correct integer value in Time", TimeDurationTextBox.Text));


            float amount;
            if (float.TryParse(AmountTextBox.Text, out amount))
            {
                if (UnitCheckBox.IsChecked == false)
                {
                    hop.Bitterness = (int)Math.Round(amount,0);
                    hop.AmountUnit = HopAmountUnitE.IBU;
                }
                else
                {
                    hop.Amount = Math.Round(amount, 2);
                    hop.AmountUnit = HopAmountUnitE.GRAMS_PER_LITER;
                }
                    
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
                UnitCheckBox.IsChecked = true;
                UnitCheckBox.IsEnabled = false;
                AmountLabel.Content = "Amount [g/L] :";

            }
            else
            {
                TimeDuration.Content = "Time[minutes from boil end]:";
                UnitCheckBox.IsEnabled = true;
            }
        }

        private void UnitCheckBox_Click(object sender, RoutedEventArgs e)
        {
            if (UnitCheckBox.IsChecked == false) {
                AmountLabel.Content = "Bitterness [IBU] :";
            }
            else
            {
                AmountLabel.Content = "Amount [g/L] :";
            }

        }
    }
}

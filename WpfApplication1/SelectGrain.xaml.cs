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
using GFCalc.DataModel;
using GFCalc.Repos;
using Grainsim.Domain;

namespace GFCalc
{
    /// <summary>
    /// Interaction logic for SelectGrain.xaml
    /// </summary>
    public partial class SelectGrain : Window
    {
        public GristPart Result { set; get; }
        public double CurrentPercentage { set; get; }

        public SelectGrain(FermentableRepository aRepo, double aCurrentPercentage)
        {
            InitializeComponent();

            CurrentPercentage = aCurrentPercentage;
            var grainsRepo = aRepo;
            var grains = grainsRepo.Get();
            comboBox.ItemsSource = grains;
            StageComboBox.ItemsSource = Enum.GetValues(typeof(FermentableStage)).Cast<FermentableStage>();
            StageComboBox.SelectedIndex = 1;
        }


        public SelectGrain(FermentableRepository aRepo, double aCurrentPercentage, GristPart aInitalGristPart)
        {
            InitializeComponent();

            CurrentPercentage = aCurrentPercentage;
            var grainsRepo = aRepo;
            var grains = grainsRepo.Get();
            comboBox.ItemsSource = grains;

            comboBox.SelectedValue = grains.FirstOrDefault(x => x.Name.Equals(aInitalGristPart.FermentableAdjunct.Name));
            textBox.Text = aInitalGristPart.Amount.ToString();
            StageComboBox.SelectedItem = aInitalGristPart.Stage;
        }



        private void button_Click(object sender, RoutedEventArgs e)
        {
            Result = new GristPart();

            FermentableAdjunct var = (FermentableAdjunct)comboBox.SelectedItem;
            if (var == null)
            {
                MessageBox.Show("Please select a grain in the drop down menu");
                return;
            }

            Result.Stage = (FermentableStage)StageComboBox.SelectedItem;

            Result.FermentableAdjunct = var;

            double part;
            if (double.TryParse(textBox.Text, out part))
            {
                if ((part + CurrentPercentage) > 100)
                    part = 100.0 - CurrentPercentage;
                Result.Amount = (int)(Math.Round(part));
                this.Close();
            }
            else
                MessageBox.Show(String.Format("Unable to interperate {0} as a decmila value. Please proviode a correct decmial value in Part", textBox.Text));
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }

}

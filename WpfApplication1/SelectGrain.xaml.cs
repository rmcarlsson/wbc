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
using WpfApplication1.Domain;

namespace GFCalc
{
    /// <summary>
    /// Interaction logic for SelectGrain.xaml
    /// </summary>
    public partial class SelectGrain : Window
    {
        public GristPart Result { set; get; }
        public float CurrentPercentage { set; get; }

        public SelectGrain(float aCurrentPercentage)
        {
            InitializeComponent();

            CurrentPercentage = aCurrentPercentage;
            var grainsRepo = new FermentableRepository();
            var grains = grainsRepo.Get();
            Result = new GristPart();
            comboBox.ItemsSource = grains;
        }


        private void button_Click(object sender, RoutedEventArgs e)
        {
            FermentableAdjunctSerializable var = (FermentableAdjunctSerializable)comboBox.SelectedItem;
            if (var == null)
            {
                MessageBox.Show("Please select a grain in the dowp down menu");
                return;
            }

            Result.FermentableAdjunct = var;

            float part;
            if (float.TryParse(textBox.Text, out part))
            {
                if ((part + CurrentPercentage) > 100)
                    part = 100f - CurrentPercentage;
                Result.Amount = part;
                this.Close();
            }
            else
                MessageBox.Show(String.Format("Unable to interperate {0} as a decmila value. Please proviode a correct decmial value in Part", textBox.Text));
        }

    }

}

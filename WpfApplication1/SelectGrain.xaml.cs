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
using WpfApplication1.DataModel;
using WpfApplication1.Repos;

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for SelectGrain.xaml
    /// </summary>
    public partial class SelectGrain : Window
    {

        public SelectGrain()
        {
            InitializeComponent();

            var grainsRepo = new FermentableRepository();
            var grains = grainsRepo.Get();
            Result = new GristPart();
            comboBox.ItemsSource = grains;
        }


        private void button_Click(object sender, RoutedEventArgs e)
        {
            FermentableAdjunct var = (FermentableAdjunct)comboBox.SelectedItem;
            if (var == null)
            {
                MessageBox.Show("Please select a grain in the dowp down menu");
                return;
            }

            Result.GristName = var.Name;

            decimal part;
            if (Decimal.TryParse(textBox.Text, out part))
            {
                Result.Share = part;
                this.Close();
            }
            else
                MessageBox.Show(String.Format("Unable to interperate {0} as a decmila value. Please proviode a correct decmial value in Part", textBox.Text));
        }

        public GristPart Result { set; get; }

    }

    public class GristPart
    {
        public string GristName { set; get; }
        public decimal Share { set; get; }
    }


}

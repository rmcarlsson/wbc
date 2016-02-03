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
using WpfApplication1.Domain;

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for SelectHops.xaml
    /// </summary>
    public partial class SelectHops : Window
    {
        public HopAddition hop { set; get; }
        public SelectHops()
        {
            InitializeComponent();

            hop = new HopAddition();
            var hopsRepo = new HopsRepo();
            var hops = hopsRepo.Get();
            comboBox.ItemsSource = hops;
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            Hops var = (Hops)comboBox.SelectedItem;
            if (var == null)
            {
                MessageBox.Show("Please select a hop in the dowp down menu");
                return;
            }
            hop.Hop = var;

            float amount;
            if (float.TryParse(AmountTextBox.Text, out amount))
            {
                hop.Amount = amount;
                this.Close();
            }
            else
                MessageBox.Show(String.Format("Unable to interperate {0} as a decmila value. Please proviode a correct decmial value in Part", AmountTextBox.Text));


        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}

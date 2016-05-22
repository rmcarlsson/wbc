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

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class MashEffeciency : Window
    {
        private double volume;
        private double gravity;

        public double Volume
        {

            private set
            {
                volume = value;
            }
            get
            {
                return volume;
            }
        }

        public double Gravity
        {

            private set
            {
                gravity = value;
            }
            get
            {
                return gravity;
            }
        }

        public MashEffeciency()
        {
            InitializeComponent();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            volume = 0;
            if (!double.TryParse(textBoxPreBoilVolume.Text, out volume))
            {
                MessageBox.Show("Please provide a valid number for pre-boil volume");
                return;
            }
            gravity = 0;
            if (!double.TryParse(textBoxPreBoilGravity.Text, out gravity))
            {
                MessageBox.Show("Please provide a valid number for pre-boil gravity");
                return;
            }

            this.Close();

        }

    }
}

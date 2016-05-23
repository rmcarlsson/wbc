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
        private int mashEfficiency;

        public int MashEfficiency
        {

            private set
            {
                mashEfficiency = value;
            }
            get
            {
                return mashEfficiency;
            }
        }

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

        public MashEffeciency(int aMashEfficiency)
        {
            InitializeComponent();

            radioButtonME.IsChecked = true;

            textBoxMashEfficiency.Text = aMashEfficiency.ToString();


            textBoxPreBoilVolume.IsEnabled = false;
            textBoxPreBoilGravity.IsEnabled = false;
            textBoxPreBoilVolume.Text = String.Empty;
            textBoxPreBoilGravity.Text = String.Empty;
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            if (radioButtonVG.IsChecked == true)
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
            }

            if (radioButtonME.IsChecked == true)
            {
                string msg = null;
                if (!int.TryParse(textBoxMashEfficiency.Text, out mashEfficiency))
                {
                    msg = "Please provide a valid number for mash efficiency";
                }

                if (mashEfficiency < 0 || mashEfficiency > 100)
                    msg = "Mash efficiency is a value between 0 - 100";

                if(msg != null)
                {
                    MessageBox.Show(msg);
                    mashEfficiency = 0;
                    return;
                }

            }

            this.Close();

        }

        private void radioButtonVG_Checked(object sender, RoutedEventArgs e)
        {
            radioButtonME.IsChecked = false;

            textBoxPreBoilVolume.IsEnabled = true;
            textBoxPreBoilGravity.IsEnabled = true;


            textBoxMashEfficiency.IsEnabled = false;
            textBoxMashEfficiency.Text = String.Empty;
            mashEfficiency = 0;


        }

        private void radioButtonME_Checked(object sender, RoutedEventArgs e)
        {
            radioButtonVG.IsChecked = false;

            textBoxMashEfficiency.IsEnabled = true;

            textBoxPreBoilVolume.IsEnabled = false;
            textBoxPreBoilGravity.IsEnabled = false;
            textBoxPreBoilVolume.Text = String.Empty;
            textBoxPreBoilGravity.Text = String.Empty;
            volume = 0;
            gravity = 0;
        }
    }
}

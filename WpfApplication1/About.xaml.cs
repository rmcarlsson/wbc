using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
    /// Interaction logic for About.xaml
    /// </summary>
    public partial class About : Window
    {
        public About()
        {
            InitializeComponent();

            Assembly thisAssem = typeof(About).Assembly;
            AssemblyName thisAssemName = thisAssem.GetName();

            Version ver = thisAssemName.Version;

            textBox.Text = String.Format(@"{0} version {1}.

Grainsim is a simple tool for receipe creation for Grainfather.

If you want to contribute please contact r_m_carlsson@hotmail.com", thisAssemName.Name, ver);

        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void donateButton_Click(object sender, RoutedEventArgs e)
         
        {
            string url = "";

            string business = "rmcarlsson@gmail.com";  // your paypal email
            string description = "Donation";            // '%20' represents a space. remember HTML!
            string country = "SE";                  // AU, US, etc.
            string currency = "EUR";                 // AUD, USD, etc.

            url += "https://www.paypal.com/cgi-bin/webscr" +
                "?cmd=" + "_donations" +
                "&business=" + business +
                "&lc=" + country +
                "&item_name=" + description +
                "&currency_code=" + currency +
                "&bn=" + "PP%2dDonationsBF";

            System.Diagnostics.Process.Start(url);
        }
    }
}

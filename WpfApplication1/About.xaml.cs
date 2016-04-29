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

    }
}

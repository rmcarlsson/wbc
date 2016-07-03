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
    /// Interaction logic for StopBrewingProcessWindow.xaml
    /// </summary>
    public partial class StopBrewingProcessWindow : Window
    {
        public bool Abort = false;
        public StopBrewingProcessWindow()
        {
            InitializeComponent();
        }

        private void AbortButton_Click(object sender, RoutedEventArgs e)
        {
            Abort = true;
            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}

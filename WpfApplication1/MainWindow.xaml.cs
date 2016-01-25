using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<GristPart> grist { set; get; }

        public MainWindow()
        {
            InitializeComponent();

            grist = new ObservableCollection<GristPart>();
            listView.ItemsSource = grist;
        }

        private void addGrains_Click(object sender, RoutedEventArgs e)
        {
            var sw = new SelectGrain();
            sw.ShowDialog();
            AddToGrist(sw.Result);
        }

        public void AddToGrist(GristPart aGristPart)
        {
            grist.Add(aGristPart);
        }
    }

}

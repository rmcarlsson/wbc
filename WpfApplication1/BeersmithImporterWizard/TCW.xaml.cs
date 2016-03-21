using GFCalc.DataModel;
using GFCalc.Repos;
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

namespace WpfApplication1.BeersmithImporterWizard
{
    /// <summary>
    /// Interaction logic for TCW.xaml
    /// </summary>
    public partial class TCW : Window
    {
        public ObservableCollection<FermentableAdjunct> Fermentables { set; get; }
        private FermentableRepository Repo;
        private string RecipeName;
        private string v;

        public TCW(string aRecipeName, FermentableRepository aMaltRepo)
        {
            InitializeComponent();
            RecipeName = aRecipeName;
            this.Repo = aMaltRepo;

            Fermentables = new ObservableCollection<FermentableAdjunct>(aMaltRepo.Get());
            //MaltsListView.ItemsSource = Fermentables;
        }

        private void ChangeTabItem(int aChange)
        {
            int newTabIndex = tabControl.SelectedIndex + aChange;
            if (newTabIndex >= tabControl.Items.Count)
            {
                newTabIndex = 0;
            }
            else if (newTabIndex < 0)
            {
                newTabIndex = tabControl.Items.Count - 1;
            }

            tabControl.SelectedIndex = newTabIndex;
        }

        private void PreviousButton_Click(object sender, RoutedEventArgs e)
        {
            ChangeTabItem(-1);
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            ChangeTabItem(1);
        }

        private void FinishButton_Click(object sender, RoutedEventArgs e)
        {
            ChangeTabItem(-tabControl.Items.Count);
        }
    }
}

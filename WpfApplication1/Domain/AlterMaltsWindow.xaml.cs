using GFCalc.DataModel;
using GFCalc.Repos;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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

namespace Grainsim.Domain
{
    /// <summary>
    /// Interaction logic for AlterMaltsWindow.xaml
    /// </summary>
    public partial class AlterMaltsWindow : Window
    {

        public FermentableRepository Repo { set; get; }
        public ObservableCollection<FermentableAdjunct> Fermentables { set; get; }

        public AlterMaltsWindow(FermentableRepository aRepo)
        {
            InitializeComponent();

            Repo = aRepo;
            Fermentables = new ObservableCollection<FermentableAdjunct>();
            var fList = Repo.Get();
            foreach (FermentableAdjunct x in fList)
                Fermentables.Add(x);

            listView.ItemsSource = Fermentables;

            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(listView.ItemsSource);
            view.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
        }

        private void DoneButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            string msg = null;
            var f = new FermentableAdjunct();
            f.Name = NameTextBox.Text;
            f.Origin = OriginTextBox.Text;
            f.MashNeeded = (MashNeededCheckBox.IsChecked == true);
            f.AdjuctType = TypeComboBox.Text;


            float floatVal = 0;
            if (float.TryParse(PotentialTextBox.Text, out floatVal))
                f.Potential = floatVal;
            else
                msg = "Please provide a valid float value for Potential";

            int intVal = 0;
            if (int.TryParse(ColorTextBox.Text, out intVal))
                f.Color = intVal;
            else
                msg = "Please provide a valid integer value for Color";

            if (int.TryParse(MaxPartTextBox.Text, out intVal))
                f.MaxPart = intVal;
            else
                msg = "Please provide a valid integer value for Max. part";

            if (msg != null)
            {
                MessageBox.Show(msg);
                return;
            }

            Repo.AddFermentable(f);

            Fermentables.Clear();
            var fList = Repo.Get();
            foreach (FermentableAdjunct x in fList)
                Fermentables.Add(x);

            listView.ItemsSource = Fermentables;

            // Reset GUI
            AddButton.Content = "Add";
            NameTextBox.Text = String.Empty;
            OriginTextBox.Text = String.Empty;
            MashNeededCheckBox.IsChecked = false;
            PotentialTextBox.Text = String.Empty;
            ColorTextBox.Text = String.Empty;
            MaxPartTextBox.Text = String.Empty;
            TypeComboBox.Text = String.Empty;


        }

        private void listView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var v =  (ListView)sender;
            if (listView.SelectedIndex >= Fermentables.Count() || 
                listView.SelectedIndex < 0 ||
                listView.SelectedItem == null)
                return;


            AddButton.Content = "Update";

            FermentableAdjunct f = (FermentableAdjunct)listView.SelectedItem;

            NameTextBox.Text = f.Name;
            OriginTextBox.Text = f.Origin;
            MashNeededCheckBox.IsChecked = (f.MashNeeded == true);
            TypeComboBox.Text = f.AdjuctType;

            PotentialTextBox.Text = f.Potential.ToString();
            ColorTextBox.Text = f.Color.ToString();
            MaxPartTextBox.Text = f.MaxPart.ToString();



        }

        private void listView_KeyDown(object sender, KeyEventArgs e)
        {
            if (Key.Delete == e.Key)
            {

                foreach (FermentableAdjunct listViewItem in ((ListView)sender).SelectedItems)
                {
                    Repo.RemoveFermentable(listViewItem);

                    Fermentables.Clear();
                    var fList = Repo.Get();
                    foreach (FermentableAdjunct x in fList)
                        Fermentables.Add(x);

                    listView.ItemsSource = Fermentables;
                    break;
                }
            }
        }

    }
}

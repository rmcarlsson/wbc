using GFCalc.DataModel;
using GFCalc.Repos;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace WpfApplication1.Domain
{
    /// <summary>
    /// Interaction logic for AddHopsWindow.xaml
    /// </summary>
    public partial class AddHopsWindow : Window
    {
        public FermentableRepository Repo { set; get; }
        public ObservableCollection<FermentableAdjunct> Fermentables { set; get; }

        public AddHopsWindow(FermentableRepository aRepo)
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

            if (AddButton.Content.Equals("Update"))
                Repo.AddFermentable(f, true);
            else
                Repo.AddFermentable(f, false);

            Fermentables.Add(f);

            AddButton.Content = "Add";


        }

        private void listView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (listView.SelectedIndex >= Fermentables.Count() || listView.SelectedIndex < 0)
                return;

            AddButton.Content = "Update";

            var f = Fermentables.ToArray()[listView.SelectedIndex];

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
                    Fermentables.Remove(listViewItem)
                    MessageBox.Show(String.Format("Shit, want to remove {0}?", listViewItem.Name));
                    break;
                }
            }
        }
    }
}

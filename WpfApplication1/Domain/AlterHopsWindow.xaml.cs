using GFCalc.DataModel;
using GFCalc.Repos;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Interaction logic for AlterHopsWindow.xaml
    /// </summary>
    public partial class AlterHopsWindow : Window
    {
        public HopsRepository Repo { set; get; }
        public ObservableCollection<Hops> Hopses { set; get; }


        public AlterHopsWindow(HopsRepository aRepo)
        {
            InitializeComponent();

            Repo = aRepo;
            Hopses = new ObservableCollection<Hops>();
            var fList = Repo.Get();
            foreach (Hops x in fList)
                Hopses.Add(x);

            listView.ItemsSource = Hopses;

            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(listView.ItemsSource);
            view.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
        }

        private void listView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (listView.SelectedIndex >= Hopses.Count() || listView.SelectedIndex < 0)
                return;

            AddButton.Content = "Update";
            var h = (Hops)listView.SelectedItem;
            NameTextBox.Text = h.Name;
            CountryTextBox.Text = h.Country;
            SubstTextBox.Text = h.Substitutes;
            PurposeComboBox.Text = h.Purpose;
            AlphaAcidTextBox.Text = h.AlphaAcid.ToString();
            BetaAcidTextBox.Text = h.BetaAcid.ToString();
            coHumTextBox.Text = h.CoHumulone.ToString();

        }

        private void listView_KeyDown(object sender, KeyEventArgs e)
        {
            if (Key.Delete == e.Key)
            {

                foreach (Hops listViewItem in ((ListView)sender).SelectedItems)
                {
                    Repo.RemoveHops(listViewItem);
                    RefreashListview();
                    break;
                }
            }
        }

        private bool ParseMaxMinAcid(HopAcids aAcid, string aValueString)
        {
            Regex r = new Regex(@"[0-9]+[\,\.]?[0-9]?");
            var m = r.Matches(aValueString);
            float floatVal;
            bool ret = true;
            int ix = 0;
            foreach (Match s in m)
            {
                if (float.TryParse(s.Value, out floatVal))
                {
                    if (ix != 0)
                        aAcid.Max = floatVal;
                    else
                        aAcid.Min = floatVal;
                }
                else
                {
                    ret = false;
                    break;
                }
                ix++;
            }

            return ret;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            string msg = null;
            var h = new Hops();

            h.Name = NameTextBox.Text;
            h.Country = CountryTextBox.Text;
            h.Substitutes = SubstTextBox.Text;
            h.Purpose = PurposeComboBox.Text;


            if (!ParseMaxMinAcid(h.AlphaAcid, AlphaAcidTextBox.Text))
                msg = "Please state a valid float value for Alpha acid";

            if (!ParseMaxMinAcid(h.BetaAcid, BetaAcidTextBox.Text))
                msg = "Please state a valid float value for Beta acid";


            if (!ParseMaxMinAcid(h.CoHumulone, coHumTextBox.Text))
                msg = "Please state a valid float value for cohumulone";

            if (msg != null)
            {
                MessageBox.Show(msg);
                return;
            }

            try {
                Repo.AddHops(h);

                RefreashListview();

            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message);
            }


            // Reset GUI
            AddButton.Content = "Add";

            NameTextBox.Text = String.Empty;
            coHumTextBox.Text = String.Empty;
            BetaAcidTextBox.Text = String.Empty;
            AlphaAcidTextBox.Text = String.Empty;
            CountryTextBox.Text = String.Empty;
            SubstTextBox.Text = String.Empty;
            PurposeComboBox.Text = String.Empty; 
        }

        private void DoneButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void RefreashListview()
        {

            Hopses.Clear();
            var fList = Repo.Get();
            foreach (Hops x in fList)
                Hopses.Add(x);

            listView.ItemsSource = Hopses;
        }
    }
}

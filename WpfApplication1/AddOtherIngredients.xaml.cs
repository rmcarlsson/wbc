using GFCalc.Domain;
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
    /// Interaction logic for AddOtherIngredients.xaml
    /// </summary>
    public partial class AddOtherIngredients : Window
    {
        public OtherIngredient Result { get; set; }
        public AddOtherIngredients()
        {
            InitializeComponent();

        }

        private void DoneButton_Click(object sender, RoutedEventArgs e)
        {
            double amount = 0;
            if (!double.TryParse(AmountTextBox1.Text, out amount))
            {
                MessageBox.Show("Please provide a valid float value for amount");
                return;
            }

            Result = new OtherIngredient();
            Result.Name = NameTextBox.Text;
            Result.Amount = amount;
            Result.Notes = NotesTextBox.Text;
            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}

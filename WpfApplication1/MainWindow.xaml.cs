﻿using System;
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

        private void listView_KeyDown(object sender, KeyEventArgs e)
        {
            if (Key.Delete == e.Key)
            {

                foreach (GristPart listViewItem in ((ListView)sender).SelectedItems)
                {
                    grist.Remove(listViewItem);
                    MessageBox.Show(String.Format("Shit, want to remove {0}. That is {1} away", listViewItem.GristName, listViewItem.Share));
                    break;
                }
            }
        }

        //public void grainBillTextBox_DragLeave

        public void AddToGrist(GristPart aGristPart)
        {
            grist.Add(aGristPart);
        }

        private Double grainBillSize;
        private Double totalMashVolume;



        private void grainBillTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void grainBillTextBox_MouseLeave(object sender, MouseEventArgs e)
        {
            grainBillTextBox = (TextBox)(sender);
            Double size;
            if (Double.TryParse(grainBillTextBox.Text, out size))
            {
                grainBillSize = size;
                totalMashVolume = size * 2.7 + 3.5;
                labelTotalMashVolume.Content = String.Format("Total mash volume [L]: {0}L", totalMashVolume);
            }
        }
    }

}

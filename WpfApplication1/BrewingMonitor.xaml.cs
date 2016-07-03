using System;
using System.Windows;
using System.Windows.Threading;
using Grpc.Core;
using Grpcproto;
using Google.Protobuf.WellKnownTypes;
using System.Net;
using System.Collections.ObjectModel;
using System.Linq;

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for BrewingMonitor.xaml
    /// </summary>
    public partial class BrewingMonitor : Window
    {
        public ObservableCollection<GFCalc.Domain.MashProfileStep> MashProfileList { get; set; }
        private IPAddress ipAddr;
        private DispatcherTimer dispatcherTimer;

        public BrewingMonitor(IPAddress aIpAddr)
        {
            InitializeComponent();

            ipAddr = aIpAddr;

            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 5);
            dispatcherTimer.Start();

            MashProfileList = new ObservableCollection<GFCalc.Domain.MashProfileStep>();
            MashStepListView.ItemsSource = MashProfileList;
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            var status = GrainbrainNetDiscovery.GetGrainBrainStatus();

            switch (status.State)
            {
                case BrewStep.Boiling:
                    handleBoiling(status);
                    break;
                case BrewStep.Heating:
                case BrewStep.MashDoneStartSparge:
                case BrewStep.Mashing:
                case BrewStep.StrikeWaterTempReached:
                    handleMashing(status);
                    break;
                default:
                    handleIdle(status);
                    break;
            }
        }

        private void handleIdle(GrainBrainStatus aStatus)
        {
            TimerLabel.IsEnabled = false;

            TempLabel.Content = String.Format("Temperature [C]: -");
            StateLabel.Content = "State - Idle";   
        }

        private void handleMashing(GrainBrainStatus aStatus)
        {

            TimerLabel.IsEnabled = false;

            MashProfileList.Clear();
            foreach (GFCalc.Domain.MashProfileStep ms in aStatus.RemainingMashStepList)
            {
                MashProfileList.Add(ms);
            }

            TempLabel.Content = String.Format("Mash temperature [C]: {0}", aStatus.Temperature);
            if (aStatus.State == BrewStep.Heating)
                StateLabel.Content = "State - Heating";
            else
                StateLabel.Content = "State - Mashing";

        }
    

        private void handleBoiling(GrainBrainStatus aStatus)
        {

            TimerLabel.IsEnabled = true;


            TempLabel.Content = String.Format("Temperature [C]: {0}", aStatus.Temperature);
            TimerLabel.Content = String.Format("Remaining boil time [min]: {0}", aStatus.RemainingBoilTime);
            if (aStatus.State == BrewStep.Heating)
                StateLabel.Content = "State - Heating";
            else
                StateLabel.Content = "State - Boiling";

        }

        private void DoneButton_Click(object sender, RoutedEventArgs e)
        {
            dispatcherTimer.Tick -= dispatcherTimer_Tick;
            dispatcherTimer.Stop();
            this.Close();
        }
    }
}

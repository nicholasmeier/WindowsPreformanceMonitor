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
using System.Windows.Navigation;
using System.Windows.Shapes;
using OpenHardwareMonitor.Hardware;
using System.Threading;
using System.ComponentModel;

namespace WindowsPerformanceMonitor
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            ComputerStatsMonitor provider = new ComputerStatsMonitor();
            Thread computerStatsThread = new Thread(new ThreadStart(provider.getComputerInformation));
            computerStatsThread.IsBackground = true;
            computerStatsThread.Start();
            Globals.SetProvider(provider);
            InitializeComponent();
        }

        private void WindowsPerformance_Closing(object sender, CancelEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to exit?", "Exit Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.No)
            {
                e.Cancel = true;
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Uc1Tab1Data1_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}

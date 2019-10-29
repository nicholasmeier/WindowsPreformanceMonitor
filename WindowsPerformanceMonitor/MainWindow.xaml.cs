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
        System.Windows.Forms.NotifyIcon ni;
        public MainWindow()
        {
            ComputerStatsMonitor provider = new ComputerStatsMonitor();
            Thread computerStatsThread = new Thread(new ThreadStart(provider.getComputerInformation));
            computerStatsThread.IsBackground = true;
            computerStatsThread.Start();
            Globals.SetProvider(provider);
            InitializeComponent();

            ni = new System.Windows.Forms.NotifyIcon();
            ni.Icon = new System.Drawing.Icon("../../Graphics/WindowsPerformanceMonitor.ico");
            ni.Visible = true;
            ni.DoubleClick +=
                delegate (object sender, EventArgs args)
                {
                    this.Show();
                    this.WindowState = WindowState.Normal;
                };
            ni.MouseDown += new System.Windows.Forms.MouseEventHandler(NotifyIcon_MouseDown);
        }

        private void WindowsPerformance_Closing(object sender, CancelEventArgs e)
        {
            this.Hide();
            ni.Visible = true;
            e.Cancel = true;

        }
        void NotifyIcon_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                var menu = this.FindResource("NotifierContextMenu") as ContextMenu;
                menu.IsOpen = true;
            }
        }

        protected void Menu_Exit(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to exit?", "Exit Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                ni.Visible = false;
                Application.Current.Shutdown();
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

using System;
using System.Windows;
using System.Windows.Controls;
using System.Threading;
using System.ComponentModel;
using System.Collections.Generic;

namespace WindowsPerformanceMonitor
{
    public partial class MainWindow : Window
    {
        System.Windows.Forms.NotifyIcon ni;
        ComputerStatsMonitor provider;
        Thread computerStatsThread;
        Thread checkForLogThread;
        public MainWindow()
        {
            provider = new ComputerStatsMonitor();
            computerStatsThread = new Thread(new ThreadStart(provider.getComputerInformation));
            computerStatsThread.IsBackground = true;
            computerStatsThread.Start();
            Globals.SetProvider(provider);
            Globals._log = new Log();
            Globals.Settings = new UserSettings();
            if (Globals.Settings.Exists())
            {
                Globals.Settings.Load();
            }
            InitializeComponent();
            ni = new System.Windows.Forms.NotifyIcon();
            ni.Icon = new System.Drawing.Icon("../../Graphics/WindowsPerformanceMonitor.ico");
            ni.Visible = true;
            ni.DoubleClick +=
                delegate (object sender, EventArgs args)
                {
                    this.Show();
                    this.WindowState = WindowState.Normal;
                    
                    if (App.Current.Properties["SUSPENDED"] != null)
                    {
                        bool sus = (bool)App.Current.Properties["SUSPENDED"];
                        if (sus == true)
                        {
                            computerStatsThread.Resume();
                            App.Current.Properties["SUSPENDED"] = false;
                        }
                    }
                    
                };
            ni.MouseDown += new System.Windows.Forms.MouseEventHandler(NotifyIcon_MouseDown);
        }

        private void WindowsPerformance_Closing(object sender, CancelEventArgs e)
        {
            this.Hide();
            // check to see if there are scheduled logs
            if (checkScheduledLogs() == 1)
            {
                // there are scheduled logs to be checked
            }else
            {
                // making the application "hibernate" add a flag for the options page to disable
                App.Current.Properties["SUSPENDED"] = true;
                computerStatsThread.Suspend();
            }

            ni.Visible = true;
            e.Cancel = true;

        }

        private int checkScheduledLogs()
        {
            List<Tuple<string, string>> logList = (List<Tuple<string, string>>)App.Current.Properties["ScheduledLogList"];
            if (logList != null)
            {
                return 1;
            }else
            {
                return 0;
            }
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

        public void Changed_Tab(object sender, EventArgs e)
        {
            provider.tabIndex = tabControl.SelectedIndex;
        }

    }
}

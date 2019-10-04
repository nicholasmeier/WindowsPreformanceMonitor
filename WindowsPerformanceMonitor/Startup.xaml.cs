using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using WindowsPerformanceMonitor.Models;

namespace WindowsPerformanceMonitor
{
    /// <summary>
    /// Interaction logic for Startup.xaml
    /// </summary>
    public partial class Startup : UserControl
    {
        private MainWindow mainWindow = null; // Reference to the MainWindow 

        //public ObservableCollection<ProcessEntry> _startupListView { get; set; }

        public Startup()
        {
            InitializeComponent();
            //_startupListView = new ObservableCollection<ProcessEntry>();
            this.DataContext = new WindowsViewModel();
        }

        // Get a reference to main windows when it is available.
        // The Loaded Event is set in the XAML code above.
        private void OnControlLoaded(object sender, RoutedEventArgs e)
        {
            mainWindow = Window.GetWindow(this) as MainWindow;
        }

        // Class for startup applications
        public class WindowsViewModel
        {
            private ObservableCollection<ApplicationStartup> m_Rows;

            public ObservableCollection<ApplicationStartup> Rows
            {
                get { return m_Rows; }
                set { m_Rows = value; }
            }

            public WindowsViewModel()
            {
                Rows = new ObservableCollection<ApplicationStartup>();

                RegistryKey localKey;
                if (Environment.Is64BitOperatingSystem)
                    localKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
                else
                    localKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32);

                /*string[] apps = localKey.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", false).GetValueNames();

                for (int i=0;i<apps.Length;i++)
                {
                    Rows.Add(new ApplicationStartup
                    {
                        Name = (string)apps.GetValue(i),
                        Status = "Enabled"
                    });
                }*/

                string path = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run";
                RegistryKey subkey = localKey.OpenSubKey(path, false);
                string[] app_names = subkey.GetValueNames();

                for (int k = 0; k < app_names.Length; k++)
                {
                    string final_appname = "";
                    string app_path = (string)subkey.GetValue((string)app_names.GetValue(k));
                    string finalpath = app_path;

                    try
                    {
                        var result = from Match match in Regex.Matches(app_path, "\"([^\"]*)\"")
                                     select match.ToString();
                        foreach (var item in result)
                        {
                            finalpath = item.ToString();
                            finalpath = finalpath.Trim('"');
                        }

                        // Print the file description.
                        FileVersionInfo myFileVersionInfo = FileVersionInfo.GetVersionInfo(finalpath);
                        final_appname = myFileVersionInfo.FileDescription;
                        //MessageBox.Show(hi);


                    }
                    catch (Exception ex)
                    {
                        //MessageBox.Show((string)app_names.GetValue(k));
                        final_appname = (string)app_names.GetValue(k);
                    }

                    Rows.Add(new ApplicationStartup
                    {
                        Name = final_appname,
                        Status = "Enabled"
                    });
                }

            }
        }

        // Testing stuff
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            
            RegistryKey localKey;
            if (Environment.Is64BitOperatingSystem)
                localKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
            else
                localKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32);

            string path = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run";
            RegistryKey subkey = localKey.OpenSubKey(path, false);

            string[] app_names = subkey.GetValueNames();

            for (int k=0;k<app_names.Length;k++)
            {
                string app_path = (string)subkey.GetValue((string)app_names.GetValue(k));
                string finalpath = app_path;

                try
                {
                    var result = from Match match in Regex.Matches(app_path, "\"([^\"]*)\"")
                                 select match.ToString();
                    foreach (var item in result)
                    {
                        finalpath = item.ToString();
                        finalpath = finalpath.Trim('"');
                    }

                    // Print the file description.
                    FileVersionInfo myFileVersionInfo = FileVersionInfo.GetVersionInfo(finalpath); 
                    string hi = "File description: " + myFileVersionInfo.FileDescription;
                    MessageBox.Show(hi);


                } catch (Exception ex)
                {
                    MessageBox.Show((string)app_names.GetValue(k));
                }
            }

        }
    }
}

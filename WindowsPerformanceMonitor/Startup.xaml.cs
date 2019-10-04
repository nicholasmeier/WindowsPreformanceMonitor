using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
            private ObservableCollection<ProcessEntry> m_Rows;

            public ObservableCollection<ProcessEntry> Rows
            {
                get { return m_Rows; }
                set { m_Rows = value; }
            }

            public WindowsViewModel()
            {
                Rows = new ObservableCollection<ProcessEntry>();

                RegistryKey localKey;
                if (Environment.Is64BitOperatingSystem)
                    localKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
                else
                    localKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32);

                string[] apps = localKey.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", false).GetValueNames();

                for (int i=0;i<apps.Length;i++)
                {
                    Rows.Add(new ProcessEntry
                    {
                        Name = (string)apps.GetValue(i)
                    });
                }

             
            }
        }

        // Testing stuff
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            /*
            RegistryKey localKey;
            if (Environment.Is64BitOperatingSystem)
                localKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
            else
                localKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32);

            string path = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run";
            string[] subkey = localKey.OpenSubKey(path, false).GetValueNames();
            
            for (int k=0;k<subkey.Length;k++)
            {
                //string temp = "TEMP******: ";
                string temp = (string)localKey.GetValue("StartCN");
              
                //(string)subkey.GetValue(k)
                Console.WriteLine("$$$$: " + temp);
            }

            //MessageBox.Show("Value: " + subkey.GetValue(0));
            //Console.Write("Value: " + str_val.ToString());
            //MessageBox.Show(RegKeys64Bits.ToString());
            */
        }
    }
}

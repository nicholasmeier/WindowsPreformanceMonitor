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
using System.Windows.Navigation;
using System.Windows.Shapes;
using WindowsPerformanceMonitor.Models;

namespace WindowsPerformanceMonitor
{
    /// <summary>

    /// </summary>
    public partial class OverlaySettings : UserControl
    {
        private ProcessEntry _overlayProcess;
        private ObservableCollection<ProcessEntry> _procListComboBox;
        private MainWindow mainWindow = null; // Reference to the MainWindow
        private OverlayWindow overlay = null; //Reference to the OverlayWindow

        #region Initialization 
        public OverlaySettings()
        {
            InitializeComponent();
            this.DataContext = this;
            _procListComboBox = new ObservableCollection<ProcessEntry>();
            _overlayProcess = null;
        }
        private void OnControlLoaded(object sender, RoutedEventArgs e)
        {
            mainWindow = Window.GetWindow(this) as MainWindow;
            if (Globals.Settings.settings.ovly_sys == true)
            {
                SYS.IsChecked = true;
            }
            else
            {
                SYS.IsChecked = false;
            }

            if (Globals.Settings.settings.ovly_cpu == true)
            {
                CPU.IsChecked = true;
            } else
            {
                CPU.IsChecked = false;
            }

            if (Globals.Settings.settings.ovly_gpu == true)
            {
                GPU.IsChecked = true;
            }
            else
            {
                GPU.IsChecked = false;
            }

            if (Globals.Settings.settings.ovly_mem == true)
            {
                MEM.IsChecked = true;
            }
            else
            {
                MEM.IsChecked = false;
            }

            if (Globals.Settings.settings.ovly_dis == true)
            {
                DIS.IsChecked = true;
            }
            else
            {
                DIS.IsChecked = false;
            }

            if (Globals.Settings.settings.ovly_net == true)
            {
                NET.IsChecked = true;
            }
            else
            {
                NET.IsChecked = false;
            }

            if (Globals.Settings.settings.ovly_tcpu == true)
            {
                TempC.IsChecked = true;
            }
            else
            {
                TempC.IsChecked = false;
            }

            if (Globals.Settings.settings.ovly_tgpu == true)
            {
                TempG.IsChecked = true;
            }
            else
            {
                TempG.IsChecked = false;
            }

        }
        #endregion

        #region UI Events
        private void Sys_stat_checked(object sender, RoutedEventArgs e)
        {
            CheckBox c = (CheckBox)sender;
            if (c.IsChecked == true)
            {
                Globals.Settings.settings.ovly_sys = true;
            }
            else
            {
                Globals.Settings.settings.ovly_sys = false;
            }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox c = (CheckBox)sender;
            switch (c.Name)
            {
                case "CPU":
                    if (c.IsChecked == true)
                    {
                        Globals.Settings.settings.ovly_cpu = true;
                    }
                    else
                    {
                        Globals.Settings.settings.ovly_cpu = false;
                    }
                    break;
                case "GPU":
                    if (c.IsChecked == true)
                    {
                        Globals.Settings.settings.ovly_gpu = true;
                    }
                    else
                    {
                        Globals.Settings.settings.ovly_gpu = false;
                    }
                    break;
                case "MEM":
                    if (c.IsChecked == true)
                    {
                        Globals.Settings.settings.ovly_mem = true;
                    }
                    else
                    {
                        Globals.Settings.settings.ovly_mem = false;
                    }
                    break;
                case "DIS":
                    if (c.IsChecked == true)
                    {
                        Globals.Settings.settings.ovly_dis = true;
                    }
                    else
                    {
                        Globals.Settings.settings.ovly_dis = false;
                    }
                    break;
                case "NET":
                    if (c.IsChecked == true)
                    {
                        Globals.Settings.settings.ovly_net = true;
                    }
                    else
                    {
                        Globals.Settings.settings.ovly_net = false;
                    }
                    break;
                case "TempC":
                    if (c.IsChecked == true)
                    {
                        Globals.Settings.settings.ovly_tcpu = true;
                    }
                    else
                    {
                        Globals.Settings.settings.ovly_tcpu = false;
                    }
                    break;
                case "TempG":
                    if (c.IsChecked == true)
                    {
                        Globals.Settings.settings.ovly_tgpu = true;
                    }
                    else
                    {
                        Globals.Settings.settings.ovly_tgpu = false;
                    }
                    break;
            }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selected = (string)Overlay_ProcessListsComboBox.SelectedItem;
            if (selected != null)
            {

            }
        }

        private void StartOverlay_Click(object sender, RoutedEventArgs e)
        {
            //tray close main window, open overlaywindow
            if ((SYS.IsChecked == false) && (_overlayProcess == null))
            {
                MessageBox.Show("Error: Please choose a process to monitor or enable system statistics", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                overlay = new OverlayWindow();
                overlay.Show();
                mainWindow.Close();
                
            }
        }
        private void Save_Button_Click(object sender, RoutedEventArgs e)
        {
            Globals.Settings.Save();
        }

        #endregion
    }
}

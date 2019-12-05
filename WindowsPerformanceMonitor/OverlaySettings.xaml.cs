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
        private ProcessEntry _selectedProcessComboBox;
        private ObservableCollection<ProcessEntry> _procListComboBox { get; set; }
        private MainWindow mainWindow = null; // Reference to the MainWindow
        private OverlayWindow overlay = null; //Reference to the OverlayWindow
        public ProcessEntry system = new ProcessEntry { Name = "SYSTEM", Pid = -1 };

        #region Initialization 
        public OverlaySettings()
        {
            InitializeComponent();
            HardwareObserver observer = new HardwareObserver(UpdateValues);
            Globals.provider.Subscribe(observer);
            this.DataContext = this;
            _procListComboBox = new ObservableCollection<ProcessEntry>();
            selectedProcessComboBox = system;
            
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
            }
            else
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

        public void UpdateValues(ComputerObj comp)
        {
            this.Dispatcher.Invoke(() =>
            {
                if (!Overlay_ProcessListsComboBox.IsDropDownOpen)
                {
                    Overlay_ProcessListsComboBox.ItemsSource = new ObservableCollection<ProcessEntry>(comp.ProcessList.OrderByDescending(p => p.Cpu));
                    Overlay_ProcessListsComboBox.SelectedItem = Find(selectedProcessComboBox, procListComboBox);
                    Overlay_ProcessListsComboBox.DisplayMemberPath = "Name";
                }
            });
        }

        public ProcessEntry Find(ProcessEntry proc, ObservableCollection<ProcessEntry> list)
        {
            if (proc != null)
            {
                if (list.FirstOrDefault(p => p.Pid == proc.Pid) != null)
                {
                    return list.First(p => p.Pid == proc.Pid);
                }
            }
            return system;
        }

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
            ProcessEntry selected = (ProcessEntry)Overlay_ProcessListsComboBox.SelectedItem;
            if (selected != null)
            {
                Globals.OverlayProc = selected;
            }
            else
            {
                Globals.OverlayProc = null;
            }
        }

        private void StartOverlay_Click(object sender, RoutedEventArgs e)
        {
            //tray close main window, open overlaywindow
            if ((SYS.IsChecked == false) && (selectedProcessComboBox == null))
            {
                MessageBox.Show("Error: Please choose a process to monitor or enable system statistics", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                overlay = new OverlayWindow();
                overlay.Show();
                stopbtn.IsEnabled = true;
                strtbtn.IsEnabled = false;
            }
        }

        private void StopOverlay_Click(object sender, RoutedEventArgs e)
        {
            overlay.Close();
            stopbtn.IsEnabled = false;
            strtbtn.IsEnabled = true;
        }

        private void Save_Button_Click(object sender, RoutedEventArgs e)
        {
            Globals.Settings.Save();
        }

        private void slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            double val = Convert.ToDouble(e.NewValue);
            OpacityValue.Text = $"{Math.Round(val, 1)}";
            Globals.Settings.settings.ovly_opac = val;

        }

        #endregion

        #region ComboBox Stuff
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        public ProcessEntry selectedProcessComboBox
        {
            get { return _selectedProcessComboBox; }
            set
            {
                _selectedProcessComboBox = value;
                OnPropertyChanged(nameof(selectedProcessComboBox));
            }
        }
        public ObservableCollection<ProcessEntry> procListComboBox
        {
            get { return _procListComboBox; }
            set
            {
                _procListComboBox = value;
                OnPropertyChanged(nameof(procListComboBox));
            }

        }

        #endregion
    }
}

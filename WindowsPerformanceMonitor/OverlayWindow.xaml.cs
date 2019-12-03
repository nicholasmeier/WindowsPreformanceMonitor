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
using System.Windows.Interop;
using System.Runtime.InteropServices;
using WindowsPerformanceMonitor.Models;
using WindowsPerformanceMonitor.Backend;

namespace WindowsPerformanceMonitor
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class OverlayWindow : Window
    {
        #region Transparency
        public const int WS_EX_TRANSPARENT = 0x00000020;
        public const int GWL_EXSTYLE = (-20);

        [DllImport("user32.dll")]
        public static extern int GetWindowLong(IntPtr hwnd, int index);

        [DllImport("user32.dll")]
        public static extern int SetWindowLong(IntPtr hwnd, int index, int newStyle);

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            // Get this window's handle
            IntPtr hwnd = new WindowInteropHelper(this).Handle;

            // Change the extended window style to include WS_EX_TRANSPARENT
            int extendedStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
            SetWindowLong(hwnd, GWL_EXSTYLE, extendedStyle | WS_EX_TRANSPARENT);
        }
        #endregion
        private MainWindow mainWindow = null; //ref to main window
        private ProcessEntry _selectedProcess = null;
        private ProcessEntry system = null;
        private Hardware hw = new Hardware();

        public OverlayWindow()
        {
            this.Top = 0;
            this.Topmost = true;
            this.ShowActivated = false;
            InitializeComponent();
            HardwareObserver ol_observer = new HardwareObserver(UpdateValues);
            Globals.provider.Subscribe(ol_observer);
            #region Stat Visibility
            if (Globals.Settings.settings.ovly_sys == false)
            {
                SYS.Visibility = Visibility.Collapsed;
                SYS_CPU.Visibility = Visibility.Collapsed;
                SYS_GPU.Visibility = Visibility.Collapsed;
                SYS_MEM.Visibility = Visibility.Collapsed;
                SYS_DIS.Visibility = Visibility.Collapsed;
                SYS_NET.Visibility = Visibility.Collapsed;
            }
            else
            {
                system = new ProcessEntry { Name = "SYSTEM", Pid = -1 };
            }
            if (Globals.Settings.settings.ovly_tcpu == false)
            {
                CPU_TEMP.Visibility = Visibility.Collapsed;
            }
            if (Globals.Settings.settings.ovly_tgpu == false)
            {
                GPU_TEMP.Visibility = Visibility.Collapsed;
            }
            if (Globals.Settings.settings.ovly_cpu == false)
            {
                SYS_CPU.Visibility = Visibility.Collapsed;
                PROC_CPU.Visibility = Visibility.Collapsed;
            }
            if (Globals.Settings.settings.ovly_gpu == false)
            {
                SYS_GPU.Visibility = Visibility.Collapsed;
                PROC_GPU.Visibility = Visibility.Collapsed;
            }
            if (Globals.Settings.settings.ovly_mem == false)
            {
                SYS_MEM.Visibility = Visibility.Collapsed;
                PROC_MEM.Visibility = Visibility.Collapsed;
            }
            if (Globals.Settings.settings.ovly_dis == false)
            {
                SYS_DIS.Visibility = Visibility.Collapsed;
                PROC_DIS.Visibility = Visibility.Collapsed;
            }
            if (Globals.Settings.settings.ovly_net == false)
            {
                SYS_NET.Visibility = Visibility.Collapsed;
                PROC_NET.Visibility = Visibility.Collapsed;
            }

            if (Globals.Settings.settings.OverlayProc != null)
            {
                _selectedProcess = Globals.Settings.settings.OverlayProc;
            }
            else
            {
                PROC.Visibility = Visibility.Collapsed;
                PROC_CPU.Visibility = Visibility.Collapsed;
                PROC_GPU.Visibility = Visibility.Collapsed;
                PROC_MEM.Visibility = Visibility.Collapsed;
                PROC_DIS.Visibility = Visibility.Collapsed;
                PROC_NET.Visibility = Visibility.Collapsed;
            }
            #endregion
        }

        public void UpdateValues(ComputerObj comp)
        {
            this.Dispatcher.Invoke(() =>
            {
                if (system != null)
                {
                    updateSysStats(comp);
                }
                if (_selectedProcess != null)
                {
                    updateProcStats();
                }
                
            });
        }

        public void updateSysStats(ComputerObj comp)
        {
            SYS_CPU.Content = $"CPU: {Math.Round(comp.TotalCpu, 0)}%";
            SYS_GPU.Content = $"GPU: {Math.Round(comp.TotalGpu, 0)}%";
            SYS_MEM.Content = $"Memory: {Math.Round(comp.TotalMemory, 0)}%";
            SYS_DIS.Content = $"Disk: {Math.Round(comp.TotalDisk, 0)}%";
            SYS_NET.Content = $"Network: {Math.Round(comp.TotalNetwork, 0)}%";
            CPU_TEMP.Content = $"CPU TEMP: {Math.Round(hw.CpuTemp(comp), 1)}°";
            GPU_TEMP.Content = $"GPU TEMP: {Math.Round(hw.GpuTemp(comp), 1)}°";
        }

        public void updateProcStats()
        {
            PROC.Content = _selectedProcess.Name;
            PROC_CPU.Content = $"CPU: {Math.Round(_selectedProcess.Cpu, 0)}%";
            PROC_GPU.Content = $"GPU: {Math.Round(_selectedProcess.Gpu, 0)}%";
            PROC_MEM.Content = $"Memory: {Math.Round(_selectedProcess.Memory, 0)}%";
            PROC_DIS.Content = $"Disk: {Math.Round(_selectedProcess.Network, 0)}%";
            PROC_NET.Content = $"Network: {Math.Round(_selectedProcess.Disk, 0)}%";
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            Window w = (Window)sender;
            this.ShowActivated = false;
            w.Topmost = true;
        }
    }
}

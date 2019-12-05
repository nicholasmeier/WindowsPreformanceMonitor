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
        //colors for stat warnings
        private Color stat_good = Color.FromRgb(128, 255, 128);
        private Color stat_ok = Color.FromRgb(255, 255, 26);
        private Color stat_bad = Color.FromRgb(255, 71, 26);

        public OverlayWindow()
        {
            InitializeComponent();
            this.Top = 0;
            this.Topmost = true;
            this.ShowActivated = false;
            this.Opacity = Globals.Settings.settings.ovly_opac;
            
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

            if (Globals.OverlayProc != null)
            {
                _selectedProcess = Globals.OverlayProc;
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
            double cpu = Math.Round(comp.TotalCpu, 0);
            double gpu = Math.Round(comp.TotalGpu, 0);
            double mem = Math.Round(comp.TotalMemory, 0);
            double disk = Math.Round(comp.TotalDisk, 0);
            double net = Math.Round(comp.TotalNetwork, 0);
            double ctp = Math.Round(hw.CpuTemp(comp), 1);
            double gtp = Math.Round(hw.GpuTemp(comp), 1);
            SYS_CPU.Content = $"CPU: {cpu}%";
            SYS_GPU.Content = $"GPU: {gpu}%";
            SYS_MEM.Content = $"Memory: {mem}%";
            SYS_DIS.Content = $"Disk: {disk}%";
            SYS_NET.Content = $"Network: {net}%";
            CPU_TEMP.Content = $"CPU TEMP: {ctp}°";
            GPU_TEMP.Content = $"GPU TEMP: {gtp}°";
            
            //change background colors for stat labels
            switch(cpu){
                case double c when (c <= 50):
                    SYS_CPU.Background = new SolidColorBrush(stat_good);
                    break;
                case double c when (c >= 50 && c < 75):
                    SYS_CPU.Background = new SolidColorBrush(stat_ok);
                    break;
                case double c when (c >= 75):
                    SYS_CPU.Background = new SolidColorBrush(stat_bad);
                    break;
            }
            switch (gpu)
            {
                case double g when (g <= 50):
                    SYS_GPU.Background = new SolidColorBrush(stat_good);
                    break;
                case double g when (g >= 50 && g < 75):
                    SYS_GPU.Background = new SolidColorBrush(stat_ok);
                    break;
                case double g when (g >= 75):
                    SYS_GPU.Background = new SolidColorBrush(stat_bad);
                    break;
            }
            switch (mem)
            {
                case double c when (c <= 50):
                    SYS_MEM.Background = new SolidColorBrush(stat_good);
                    break;
                case double c when (c >= 50 && c < 75):
                    SYS_MEM.Background = new SolidColorBrush(stat_ok);
                    break;
                case double c when (c >= 75):
                    SYS_MEM.Background = new SolidColorBrush(stat_bad);
                    break;
            }
            switch (disk)
            {
                case double c when (c <= 50):
                    SYS_DIS.Background = new SolidColorBrush(stat_good);
                    break;
                case double c when (c >= 50 && c < 75):
                    SYS_DIS.Background = new SolidColorBrush(stat_ok);
                    break;
                case double c when (c >= 75):
                    SYS_DIS.Background = new SolidColorBrush(stat_bad);
                    break;
            }
            switch (net)
            {
                case double c when (c <= 50):
                    SYS_NET.Background = new SolidColorBrush(stat_good);
                    break;
                case double c when (c >= 50 && c < 75):
                    SYS_NET.Background = new SolidColorBrush(stat_ok);
                    break;
                case double c when (c >= 75):
                    SYS_NET.Background = new SolidColorBrush(stat_bad);
                    break;
            }
            switch (ctp)
            {
                case double c when (c <= 50):
                    CPU_TEMP.Background = new SolidColorBrush(stat_good);
                    break;
                case double c when (c >= 50 && c < 75):
                    CPU_TEMP.Background = new SolidColorBrush(stat_ok);
                    break;
                case double c when (c >= 75):
                    CPU_TEMP.Background = new SolidColorBrush(stat_bad);
                    break;
            }
            switch (gtp)
            {
                case double c when (c <= 50):
                    GPU_TEMP.Background = new SolidColorBrush(stat_good);
                    break;
                case double c when (c >= 50 && c < 75):
                    GPU_TEMP.Background = new SolidColorBrush(stat_ok);
                    break;
                case double c when (c >= 75):
                    GPU_TEMP.Background = new SolidColorBrush(stat_bad);
                    break;
            }
        }

        public void updateProcStats()
        {
            double cpu = Math.Round(_selectedProcess.Cpu, 0);
            double gpu = Math.Round(_selectedProcess.Gpu, 0);
            double mem = Math.Round(_selectedProcess.Memory, 0);
            double disk = Math.Round(_selectedProcess.Disk, 0);
            double net = Math.Round(_selectedProcess.Network, 0);

            PROC.Content = _selectedProcess.Name;
            PROC_CPU.Content = $"CPU: {cpu}%";
            PROC_GPU.Content = $"GPU: {gpu}%";
            PROC_MEM.Content = $"Memory: {mem}%";
            PROC_DIS.Content = $"Disk: {disk}%";
            PROC_NET.Content = $"Network: {net}%";

            //change background colors for stat labels
            switch (cpu)
            {
                case double c when (c <= 50):
                    PROC_CPU.Background = new SolidColorBrush(stat_good);
                    break;
                case double c when (c >= 50 && c < 75):
                    PROC_CPU.Background = new SolidColorBrush(stat_ok);
                    break;
                case double c when (c >= 75):
                    PROC_CPU.Background = new SolidColorBrush(stat_bad);
                    break;
            }
            switch (gpu)
            {
                case double g when (g <= 50):
                    PROC_GPU.Background = new SolidColorBrush(stat_good);
                    break;
                case double g when (g >= 50 && g < 75):
                    PROC_GPU.Background = new SolidColorBrush(stat_ok);
                    break;
                case double g when (g >= 75):
                    PROC_GPU.Background = new SolidColorBrush(stat_bad);
                    break;
            }
            switch (mem)
            {
                case double c when (c <= 50):
                    PROC_MEM.Background = new SolidColorBrush(stat_good);
                    break;
                case double c when (c >= 50 && c < 75):
                    PROC_MEM.Background = new SolidColorBrush(stat_ok);
                    break;
                case double c when (c >= 75):
                    PROC_MEM.Background = new SolidColorBrush(stat_bad);
                    break;
            }
            switch (disk)
            {
                case double c when (c <= 50):
                    PROC_DIS.Background = new SolidColorBrush(stat_good);
                    break;
                case double c when (c >= 50 && c < 75):
                    PROC_DIS.Background = new SolidColorBrush(stat_ok);
                    break;
                case double c when (c >= 75):
                    PROC_DIS.Background = new SolidColorBrush(stat_bad);
                    break;
            }
            switch (net)
            {
                case double c when (c <= 50):
                    PROC_NET.Background = new SolidColorBrush(stat_good);
                    break;
                case double c when (c >= 50 && c < 75):
                    PROC_NET.Background = new SolidColorBrush(stat_ok);
                    break;
                case double c when (c >= 75):
                    PROC_NET.Background = new SolidColorBrush(stat_bad);
                    break;
            }
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            Window w = (Window)sender;
            this.ShowActivated = false;
            w.Topmost = true;
        }
    }
}

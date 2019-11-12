using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using WindowsPerformanceMonitor.Models;

namespace WindowsPerformanceMonitor
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class SetThreshold : Window
    {
        Window mainWindowRef = null;
        ProcessEntry selectedProcess = new ProcessEntry();

        public SetThreshold(Window mainWindow, ProcessEntry process)
        {
            InitializeComponent();
            mainWindowRef = mainWindow;
            selectedProcess = process;
            loadThresholds();
            
        }

        private void loadThresholds()
        {
            if (selectedProcess.CpuThreshold < 0 || selectedProcess.CpuThreshold > 100)
            {
                selectedProcess.CpuThreshold = 0;
            }

            if (selectedProcess.GpuThreshold < 0 || selectedProcess.GpuThreshold > 100)
            {
                selectedProcess.GpuThreshold = 0;
            }

            if (selectedProcess.MemoryThreshold < 0 || selectedProcess.MemoryThreshold > 100)
            {
                selectedProcess.MemoryThreshold = 0;
            }

            CPUThresholdTextBox.Text = selectedProcess.CpuThreshold.ToString();
            GPUThresholdTextBox.Text = selectedProcess.GpuThreshold.ToString();
            MemoryThresholdTextBox.Text = selectedProcess.MemoryThreshold.ToString();
            return;
        }

        private void SaveThresholds_Click(object sender, RoutedEventArgs e)
        {
            selectedProcess.CpuThreshold = Convert.ToDouble(CPUThresholdTextBox.Text);
            selectedProcess.GpuThreshold = Convert.ToDouble(GPUThresholdTextBox.Text);
            selectedProcess.MemoryThreshold = Convert.ToDouble(MemoryThresholdTextBox.Text);
            loadThresholds();
            MessageBox.Show("The notifications thresholds were successfully updated.", "Notification Thresholds Updated", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void SetThresholdView_Closing(object sender, CancelEventArgs e)
        {
            mainWindowRef.Show();
        }
    }
}

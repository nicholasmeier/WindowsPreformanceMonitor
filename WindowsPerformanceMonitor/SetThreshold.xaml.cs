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
            if (selectedProcess.cpuThreshold < 0 || selectedProcess.cpuThreshold > 100)
            {
                selectedProcess.cpuThreshold = 0;
            }

            if (selectedProcess.gpuThreshold < 0 || selectedProcess.gpuThreshold > 100)
            {
                selectedProcess.gpuThreshold = 0;
            }

            if (selectedProcess.memoryThreshold < 0 || selectedProcess.memoryThreshold > 100)
            {
                selectedProcess.memoryThreshold = 0;
            }

            CPUThresholdTextBox.Text = selectedProcess.cpuThreshold.ToString();
            GPUThresholdTextBox.Text = selectedProcess.gpuThreshold.ToString();
            MemoryThresholdTextBox.Text = selectedProcess.memoryThreshold.ToString();
            return;
        }

        private void SaveThresholds_Click(object sender, RoutedEventArgs e)
        {
            selectedProcess.cpuThreshold = Convert.ToDouble(CPUThresholdTextBox.Text);
            selectedProcess.gpuThreshold = Convert.ToDouble(GPUThresholdTextBox.Text);
            selectedProcess.memoryThreshold = Convert.ToDouble(MemoryThresholdTextBox.Text);
            loadThresholds();
            MessageBox.Show("The notifications thresholds were successfully updated.", "Notification Thresholds Updated", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void SetThresholdView_Closing(object sender, CancelEventArgs e)
        {
            mainWindowRef.Show();
        }
    }
}

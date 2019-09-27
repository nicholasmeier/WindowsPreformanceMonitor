using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using OpenHardwareMonitor.Hardware;
using PerformanceMonitor.Cpp.CLI;
using WindowsPerformanceMonitor.Backend;
using WindowsPerformanceMonitor.Models;

namespace WindowsPerformanceMonitor
{
    /// <summary>
    /// Interaction logic for RealTime.xaml
    /// </summary>
    public partial class RealTime : UserControl
    {
        private MainWindow mainWindow = null; // Reference to the MainWindow

        public RealTime()
        {
            InitializeComponent();
            InitializeComboBox();
            Processes p = new Processes();
            List<ProcessEntry> list = p.GetProcesses();

            for (int i = 0; i < list.Count; i++)
            {
                processList.Items.Add(list[i]);
            }
        }

        public void UpdateValues(Computer comp)
        {
            return;
        }

        #region Initialization

        private void OnControlLoaded(object sender, RoutedEventArgs e)
        {
            mainWindow = Window.GetWindow(this) as MainWindow;
        }

        private void InitializeComboBox()
        {
            comboBox1.Items.Add("System");

            for (var i = 0; i < 5; i++)
            {
                comboBox1.Items.Add($"Process {i}");
            }

            comboBox1.SelectedItem = "System";
        }

        #endregion

        #region Events

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // TODO
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // TODO
        }

        private void ProcessList_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // TODO
        }

        #endregion
    }
}

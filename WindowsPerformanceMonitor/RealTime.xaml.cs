using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
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
        public ObservableCollection<ProcessEntry> procList { get; set; }

        public RealTime()
        {
            InitializeComponent();
            InitializeComboBox();
            HardwareObserver observer = new HardwareObserver(UpdateValues);
            Globals.provider.Subscribe(observer);
            procList = new ObservableCollection<ProcessEntry>();
            this.DataContext = this;
        }

        public void UpdateValues(ComputerObj comp)
        {
            UpdateList(comp.ProcessList);
            return;
        }

        public void UpdateList(ObservableCollection<ProcessEntry> procs)
        {
            this.Dispatcher.Invoke(() =>
            {
                // For some reason, just doing procList = procs won't work.
                Console.WriteLine("Updating process list");
                procList.Clear();
                for (int i = 0; i < procs.Count; i++)
                {
                    procList.Add(procs[i]);
                }

            });

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
        }

        private void ProcessList_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // TODO
        }

        private void CheckBoxChanged(object sender, RoutedEventArgs e)
        {
            if (procList != null)
            {
                procList.Add(new ProcessEntry() { Name = "Name" });

            }
        }

        #endregion
    }
}

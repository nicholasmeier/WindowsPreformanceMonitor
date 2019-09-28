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
using System.ComponentModel;
using System.Linq;

namespace WindowsPerformanceMonitor
{
    /// <summary>
    /// Interaction logic for RealTime.xaml
    /// </summary>
    public partial class RealTime : UserControl, INotifyPropertyChanged
    {
        private MainWindow mainWindow = null; // Reference to the MainWindow
        public event PropertyChangedEventHandler PropertyChanged;

        private ProcessEntry _selectedProcessListView;
        public ProcessEntry selectedProcessListView
        {
            get { return _selectedProcessListView; }
            set
            {
                _selectedProcessListView = value;
                OnPropertyChanged(nameof(selectedProcessListView));
            }
        }

        private ProcessEntry _selectedProcessComboBox;
        public ProcessEntry selectedProcessComboBox
        {
            get { return _selectedProcessComboBox; }
            set
            {
                _selectedProcessComboBox = value;
                OnPropertyChanged(nameof(selectedProcessComboBox));
            }
        }

        public ObservableCollection<ProcessEntry> _procList { get; set; }
        public ObservableCollection<ProcessEntry> procList
        {
            get { return _procList; }
            set
            {
                _procList = value;
                OnPropertyChanged(nameof(procList));
            }
        }

        public RealTime()
        {
            InitializeComponent();
            InitializeComboBox();
            HardwareObserver observer = new HardwareObserver(UpdateValues);
            Globals.provider.Subscribe(observer);
            procList = new ObservableCollection<ProcessEntry>();
            this.DataContext = this;
        }

        #region Observer Update Functions

        public void UpdateValues(ComputerObj comp)
        {
            UpdateList(comp);
            return;
        }

        public void UpdateList(ComputerObj comp)
        {
            this.Dispatcher.Invoke(() =>
            {
                ProcessEntry selectedListView = selectedProcessListView;
                ProcessEntry selectedComboBox = selectedProcessComboBox;

                procList = new ObservableCollection<ProcessEntry>(comp.ProcessList.OrderByDescending(p => p.Cpu)); // TEMPORARY - sorting to make it easier since most processes use 0%

                selectedProcessListView = Find(selectedListView);
                selectedProcessComboBox = Find(selectedComboBox);

                UpdateColumnHeaders(comp);
            });
        }

        public ProcessEntry Find(ProcessEntry proc)
        {
            if (proc != null)
            {
                if (procList.FirstOrDefault(p => p.Pid == proc.Pid) != null)
                {
                    return procList.First(p => p.Pid == proc.Pid);
                }
            }

            return null;
        }

        public void UpdateColumnHeaders(ComputerObj comp)
        {
            listView_gridView.Columns[0].Header = $"Process {comp.ProcessList.Count}";
            listView_gridView.Columns[1].Header = $"CPU {Math.Round(comp.TotalCpu, 2)}%";
            listView_gridView.Columns[2].Header = $"GPU {Math.Round(comp.TotalGpu, 2)}%";
            listView_gridView.Columns[3].Header = $"Memory {Math.Round(comp.TotalMemory, 2)}%";
            listView_gridView.Columns[4].Header = $"Disk {Math.Round(comp.TotalDisk, 2)}%";
            listView_gridView.Columns[5].Header = $"Network {Math.Round(comp.TotalNetwork, 2)}%";
        }

        #endregion

        #region Initialization

        private void OnControlLoaded(object sender, RoutedEventArgs e)
        {
            mainWindow = Window.GetWindow(this) as MainWindow;
        }

        private void InitializeComboBox()
        {
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

        }
        private void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        #endregion
    }
}

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
using System.Globalization;

namespace WindowsPerformanceMonitor
{
    /// <summary>
    /// Interaction logic for RealTime.xaml
    /// </summary>
    public partial class RealTime : UserControl, INotifyPropertyChanged
    {
        private MainWindow mainWindow = null; // Reference to the MainWindow
        private ProcessEntry _selectedProcessListView;
        private ProcessEntry _selectedProcessComboBox;
        public ObservableCollection<ProcessEntry> _procListListView { get; set; }
        public ObservableCollection<ProcessEntry> _procListComboBox { get; set; }
        public ProcessEntry system = new ProcessEntry { Name = "SYSTEM", Pid = -1 };


        #region Initialization

        public RealTime()
        {
            InitializeComponent();
            HardwareObserver observer = new HardwareObserver(UpdateValues);
            Globals.provider.Subscribe(observer);
            _procListListView = new ObservableCollection<ProcessEntry>();
            _procListComboBox = new ObservableCollection<ProcessEntry>();
            selectedProcessComboBox = system;
            this.DataContext = this;
        }
        private void OnControlLoaded(object sender, RoutedEventArgs e)
        {
            mainWindow = Window.GetWindow(this) as MainWindow;
        }

        #endregion

        #region Hardware Observer Updates

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

                procListListView = new ObservableCollection<ProcessEntry>(comp.ProcessList.OrderByDescending(p => p.Cpu)); // TEMPORARY - sorting to make it easier since most processes use 0%
                procListComboBox = new ObservableCollection<ProcessEntry>(comp.ProcessList.OrderByDescending(p => p.Cpu)); // TEMPORARY - sorting to make it easier since most processes use 0%
                procListComboBox.Insert(0, system);

                selectedProcessListView = Find(selectedListView, procListListView);
                selectedProcessComboBox = Find(selectedComboBox, procListComboBox);

                UpdateColumnHeaders(comp);
            });
        }

        public void UpdateColumnHeaders(ComputerObj comp)
        {
            listView_gridView.Columns[0].Header = $"Process {comp.ProcessList.Count}";
            listView_gridView.Columns[1].Header = $"CPU {Math.Round(comp.TotalCpu, 2)}%";
            listView_gridView.Columns[2].Header = $"GPU {Math.Round(comp.TotalGpu, 2)}%";
            listView_gridView.Columns[3].Header = $"Memory {Math.Round(comp.TotalMemory, 2)}%";
            listView_gridView.Columns[4].Header = $"Disk {Math.Round(comp.TotalDisk, 2)}%";
            listView_gridView.Columns[5].Header = $"Network {Math.Round(comp.TotalNetwork, 2)}%";

            if (Math.Round(comp.TotalCpu, 2) > 100)
            {
                listView_gridView.Columns[1].Header = $"CPU 100%";
            }
        }

        #endregion

        #region UI Events

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // TODO
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Console.WriteLine("Combobox " + comboBox1.SelectedItem);
            ProcessEntry selected = (ProcessEntry) comboBox1.SelectedItem;

            if (selected != null)
            {
                liveGraph.ProcessPid = selected.Pid;
            }
            else
            {
                liveGraph.ProcessPid = -1;
            }
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            var radioButton = sender as RadioButton;
            if (radioButton == null)
            {
                return;
            }

            liveGraph.StatToGraph = radioButton.Content.ToString();
         }


        private void ProcessList_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            
        }

        private void CheckBoxChanged(object sender, RoutedEventArgs e)
        {

        }

        #endregion

        #region INotifyPropertyChanged Events

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public ProcessEntry selectedProcessListView
        {
            get { return _selectedProcessListView; }
            set
            {
                _selectedProcessListView = value;
                OnPropertyChanged(nameof(selectedProcessListView));
            }
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

        public ObservableCollection<ProcessEntry> procListListView
        {
            get { return _procListListView; }
            set
            {
                _procListListView = value;
                OnPropertyChanged(nameof(procListListView));
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

        #region Helpers
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

        #endregion
    }
}

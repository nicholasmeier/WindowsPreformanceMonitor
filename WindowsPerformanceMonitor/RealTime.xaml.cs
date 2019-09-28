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
        public int selectedPid;
        public event PropertyChangedEventHandler PropertyChanged;

        private ProcessEntry _selectedProcessEntry;
        public ProcessEntry selectedProcessEntry
        {
            get { return _selectedProcessEntry; }
            set
            {
                _selectedProcessEntry = value;
                OnSelectedItemChanged(nameof(selectedProcessEntry));
            }
        }

        public ObservableCollection<ProcessEntry> _procList { get; set; }
        public ObservableCollection<ProcessEntry> procList
        {
            get { return _procList; }
            set
            {
                _procList = value;
                OnProccessListChanged(nameof(procList));
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

        public void UpdateValues(ComputerObj comp)
        {
            UpdateList(comp.ProcessList);
            return;
        }

        public void UpdateList(ObservableCollection<ProcessEntry> procs)
        {
            this.Dispatcher.Invoke(() =>
            {
                ProcessEntry selected = selectedProcessEntry; // Save selected item.
                procList = procs;
                if (selected != null) // Item was selected.
                {
                    if (procList.FirstOrDefault(p => p.Pid == selected.Pid) != null) // Item is in updated list.
                    {
                        selectedProcessEntry = procList.First(p => p.Pid == selected.Pid); // Reselect the item.
                    }
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
        private void OnProccessListChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private void OnSelectedItemChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        }

        #endregion
    }
}

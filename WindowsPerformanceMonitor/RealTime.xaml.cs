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
using WindowsPerformanceMonitor.Graphs;

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
        public ObservableCollection<ProcessEntry> _procListTreeView { get; set; }
        public ObservableCollection<ProcessEntry> _procListComboBox { get; set; }
        public ProcessEntry system = new ProcessEntry { Name = "SYSTEM", Pid = -1 };


        #region Initialization

        public RealTime()
        {
            InitializeComponent();
            HardwareObserver observer = new HardwareObserver(UpdateValues);
            Globals.provider.Subscribe(observer);
            _procListListView = new ObservableCollection<ProcessEntry>();
            _procListTreeView = new ObservableCollection<ProcessEntry>();
            _procListComboBox = new ObservableCollection<ProcessEntry>();
            selectedProcessComboBox = system;
            this.DataContext = this;
            cbAll.IsChecked = true;
            liveGraph.connect();
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
                //procListTreeView = new ObservableCollection<ProcessEntry>(comp.ProcessTree.OrderByDescending(p => p.Cpu));
                procListComboBox.Insert(0, system);

                selectedProcessListView = Find(selectedListView, procListListView);
                selectedProcessComboBox = Find(selectedComboBox, procListComboBox);

                UpdateColumnHeaders(comp);
                UpdateProcessTreeView();
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

        public void UpdateProcessTreeView()
        {
            // make the tree with parent, child, and subchild
            foreach (ProcessEntry parent in _procListTreeView)
            {
                TreeViewItem ParentItem = new TreeViewItem();
                ParentItem.Header = parent.Name + " " + parent.Pid;
                // check to see if they have a child to add
                if (parent.ChildProcesses.Count > 0)
                {
                    foreach (ProcessEntry child in parent.ChildProcesses)
                    {
                        TreeViewItem ChildItem = new TreeViewItem();
                        ChildItem.Header = child.Name + " " + child.Pid;
                        // check to see if they have a sub child to add
                        if (child.ChildProcesses.Count > 0)
                        {
                            foreach (ProcessEntry subchild in child.ChildProcesses)
                            {
                                //get the subchild and add it to the child
                                TreeViewItem SubChildItem = new TreeViewItem();
                                SubChildItem.Header = subchild.Name + " " + subchild.Pid;
                                ChildItem.Items.Add(SubChildItem);
                            }
                        }
                        ParentItem.Items.Add(ChildItem);
                    }
                }
                //ProcessTreeView.Items.Add(ParentItem);
            }
        }

        #endregion

        #region UI Events

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // TODO
        }

        private void ScheduleLogProcess_Click(object sender, RoutedEventArgs e)
        {
            if (listView_ProcList.SelectedIndex > -1)
            {
            }
        }

        private void KillProcess_Click(object sender, RoutedEventArgs e)
        {
            if (listView_ProcList.SelectedIndex > -1)
            {
                Processes procs = new Processes();
                ProcessEntry procEntry = (ProcessEntry)listView_ProcList.Items[listView_ProcList.SelectedIndex];
                procs.Kill(procEntry.Pid);

                List<ProcessEntry> newList = new List<ProcessEntry>();
                for (int i = 0; i < listView_ProcList.Items.Count; i++)
                {
                    ProcessEntry p = (ProcessEntry)listView_ProcList.Items[i];
                    if (p.Pid != procEntry.Pid)
                    {
                        newList.Add(p);
                    }
                }

                listView_ProcList.ItemsSource = new ObservableCollection<ProcessEntry>(newList);
            }
        }

        private void TrackInLog_Click(object sender, EventArgs e)
        {
            ProcessEntry procEntry = (ProcessEntry)listView_ProcList.Items[listView_ProcList.SelectedIndex];
            Globals._log.AddPid(procEntry.Pid);
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Console.WriteLine("Combobox " + comboBox1.SelectedItem);
            ProcessEntry selected = (ProcessEntry)comboBox1.SelectedItem;

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

            string button = radioButton.Content.ToString();
            if (radioButton.Content.ToString() == "All")
            {
                liveGraph.SeriesVisibility[(int)Series.Cpu] = true;
                liveGraph.SeriesVisibility[(int)Series.Gpu] = true;
                liveGraph.SeriesVisibility[(int)Series.Memory] = true;
                liveGraph.SeriesVisibility[(int)Series.Disk] = true;
                liveGraph.SeriesVisibility[(int)Series.Network] = true;
                liveGraph.SeriesVisibility[(int)Series.CpuTemp] = false;
                liveGraph.SeriesVisibility[(int)Series.GpuTemp] = false;
            }
            else if (button == "CPU")
            {
                SetVisibilityToFalseExecept((int)Series.Cpu);
                liveGraph.SeriesVisibility[(int)Series.Cpu] = true;
            }
            else if (button == "GPU")
            {
                SetVisibilityToFalseExecept((int)Series.Gpu);
                liveGraph.SeriesVisibility[(int)Series.Gpu] = true;
            }
            else if (button == "Memory")
            {
                SetVisibilityToFalseExecept((int)Series.Memory);
                liveGraph.SeriesVisibility[(int)Series.Memory] = true;
            }
            else if (button == "Network")
            {
                SetVisibilityToFalseExecept((int)Series.Network);
                liveGraph.SeriesVisibility[(int)Series.Network] = true;
            }
            else if (button == "Disk")
            {
                SetVisibilityToFalseExecept((int)Series.Disk);
                liveGraph.SeriesVisibility[(int)Series.Disk] = true;

            }
            else if (button == "GPU Temperature")
            {
                SetVisibilityToFalseExecept((int)Series.GpuTemp);
                liveGraph.SeriesVisibility[(int)Series.GpuTemp] = true;
            }
            else if (button == "CPU Temperature")
            {
                SetVisibilityToFalseExecept((int)Series.CpuTemp);
                liveGraph.SeriesVisibility[(int)Series.CpuTemp] = true;
            }
        }
        private void SetVisibilityToFalseExecept(int statIndex)
        {
            for (int k = 0; k < 7; k++)
            {
                if (statIndex == k) continue;
                liveGraph.SeriesVisibility[k] = false;
            }
        }
        private void ProcessList_SizeChanged(object sender, SizeChangedEventArgs e)
        {

        }

        private void CBAllChanged(object sender, RoutedEventArgs e)
        {
            bool newVal = (cbAll.IsChecked == true);
            cpu.IsChecked = newVal;
            gpu.IsChecked = newVal;
            memory.IsChecked = newVal;
            disk.IsChecked = newVal;
            network.IsChecked = newVal;
            gpuTemp.IsChecked = newVal;
            cpuTemp.IsChecked = newVal;
        }

        private void CheckBoxChanged(object sender, RoutedEventArgs e)
        {
            var checkBox = sender as CheckBox;
            if (checkBox == null)
            {
                return;
            }

            liveGraph.SeriesVisibility[(int)Series.Cpu] = (bool)cpu.IsChecked;
            liveGraph.SeriesVisibility[(int)Series.Gpu] = (bool)gpu.IsChecked;
            liveGraph.SeriesVisibility[(int)Series.Memory] = (bool)memory.IsChecked;
            liveGraph.SeriesVisibility[(int)Series.Disk] = (bool)disk.IsChecked;
            liveGraph.SeriesVisibility[(int)Series.Network] = (bool)network.IsChecked;
            liveGraph.SeriesVisibility[(int)Series.CpuTemp] = (bool)cpuTemp.IsChecked;
            liveGraph.SeriesVisibility[(int)Series.GpuTemp] = (bool)gpuTemp.IsChecked;

            //If all boxes are checked or unchecked set All correctly
            if (cpu.IsChecked == gpu.IsChecked && cpu.IsChecked == memory.IsChecked &&
                cpu.IsChecked == disk.IsChecked && cpu.IsChecked == network.IsChecked
                && cpu.IsChecked == gpuTemp.IsChecked && cpu.IsChecked == cpuTemp.IsChecked)
            {
                cbAll.IsChecked = cpu.IsChecked;
            }
            else
            {
                cbAll.IsChecked = null;

            }
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

        public ObservableCollection<ProcessEntry> procListTreeView
        {
            get { return _procListTreeView; }
            set
            {
                _procListTreeView = value;
                OnPropertyChanged(nameof(procListTreeView));
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

        private void Legend_Click(object sender, RoutedEventArgs e)
        {
            GraphLegend legend = new GraphLegend();
            legend.ShowDialog();
        }

        private void Summary_Click(object sender, RoutedEventArgs e)
        {
            SummaryView summary = new SummaryView(Application.Current.MainWindow);
            summary.Show();
            Application.Current.MainWindow.Hide();
        }

        private void General_Click(object sender, RoutedEventArgs e)
        {
            GeneralView summary = new GeneralView(Application.Current.MainWindow);
            summary.Show();
            Application.Current.MainWindow.Hide();
        }
    }
}

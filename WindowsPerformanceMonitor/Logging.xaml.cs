using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using WindowsPerformanceMonitor.Graphs;
using static WindowsPerformanceMonitor.Log;

namespace WindowsPerformanceMonitor
{
    /// <summary>
    /// Interaction logic for Logging.xaml
    /// </summary>
    /// 
    public partial class Logging : System.Windows.Controls.UserControl, INotifyPropertyChanged
    {
        private MainWindow mainWindow = null;
        private ObservableCollection<LogDetails> _logList { get; set; }
        private ObservableCollection<LogProcessEntry> _logProcList { get; set; }
        private LogDetails _selectedLog { get; set; }
        public ObservableCollection<LogProcessEntry> _procListComboBox { get; set; }
        private LogProcessEntry _selectedProcessComboBox;
        public LogProcessEntry system = new LogProcessEntry { Name = "SYSTEM", Pid = -1 };


        private Thread readThread;
        private bool paused;
        private bool back;
        public int currentLogLocation = -1;     // Current location of the log.
        public int maxLogLocation = 0;          // Max location we can be at for some log.
        ManualResetEvent pauseEvent = new ManualResetEvent(true);
        public payload log;

        public class LogDetails
        {
            public string name { get; set; }
            public string path { get; set; }
        }

        public Logging()
        {
            InitializeComponent();
            LogList = new ObservableCollection<LogDetails>();
            this.DataContext = this;
            GetLogList();
            _procListComboBox = new ObservableCollection<LogProcessEntry>();
            selectedProcessComboBox = system;
            this.DataContext = this;
            cbAll.IsChecked = true;
            procListComboBox.Insert(0, system);
            paused = false;
            StepForward.IsEnabled = false;
            StepBack.IsEnabled = false;
            readThread = new Thread(() => { });
            StartRecordingButton.IsEnabled = true;
            StopRecordingButton.IsEnabled = false;
            Globals._logRef = this;
            pBar.Value = 0;
        }

        private void OnControlLoaded(object sender, RoutedEventArgs e)
        {
            mainWindow = Window.GetWindow(this) as MainWindow;
        }

        #region Log Manager / UI Event Functions

        private void StartLog_Click(object sender, RoutedEventArgs e)
        {
            Globals._log.StartLog();
            StartRecordingButton.IsEnabled = false;
            StopRecordingButton.IsEnabled = true;
        }

        private void StopLog_Click(object sender, RoutedEventArgs e)
        {
            StopRecordingButton.IsEnabled = false;
            if (Globals._log != null)
            {
                Globals._log.WriteIt();
                GetLogList();
            }

            StartRecordingButton.IsEnabled = true;
            Globals._log = new Log();
        }

        private void PauseLog_Click(object sender, RoutedEventArgs e)
        {
            if (readThread != null)
            {
                if (!paused)
                {
                    paused = true;
                    pauseEvent.Reset();
                    PauseButton.Content = "Resume";
                    StepForward.IsEnabled = true;
                    StepBack.IsEnabled = true;
                }
                else
                {
                    if (currentLogLocation <= maxLogLocation)    // Don't let this run if we're at end.
                    {
                        paused = false;
                        pauseEvent.Set();
                        PauseButton.Content = "Pause";
                        StepForward.IsEnabled = false;
                        StepBack.IsEnabled = false;
                    }

                }
            }
        }

        private void PlayLog_Click(object sender, RoutedEventArgs e)
        {
            ResetUI();
            currentLogLocation = -1;
            if (SelectedLog != null)
            {
                readThread.Abort();

                readThread = new Thread(() =>
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        paused = false;
                        pauseEvent.Set();
                        PauseButton.Content = "Pause";
                        StepForward.IsEnabled = false;
                        StepBack.IsEnabled = false;
                    });

                    currentLogLocation = -1;
                    ConnectLog(SelectedLog.path);
                    Play(SelectedLog.path);
                });

                readThread.Start();
            }
        }

        private void ConnectLog(string path)
        {
            log = Globals._log.ReadIt(path);
            liveGraph.connect(log);
            maxLogLocation = log.mytimes.Count - 1;
            SetProgressBarMax(maxLogLocation - 2);
        }

        private void Play(string path)
        {
            while (true)
            {
                pauseEvent.WaitOne(Timeout.Infinite);

                if (currentLogLocation + 2 > maxLogLocation)
                {
                    continue;
                }

                Forward();
                NotifyLocationHasChanged();
                Thread.Sleep(50);
            }
        }

        private void Step_Forward(object sender, RoutedEventArgs e)
        {
            if (currentLogLocation < maxLogLocation)
            {
                Forward();
                NotifyLocationHasChanged();
            }
        }

        private void Step_Back(object sender, RoutedEventArgs e)
        {
            var x = currentLogLocation;
            if (currentLogLocation > 0)
            {
                Back();
                NotifyLocationHasChanged();
            }
        }

        private void Forward()
        {
            currentLogLocation++;
            IncProgressBar();
            liveGraph.Read(log, currentLogLocation);
            LogProcList = new ObservableCollection<LogProcessEntry>(log.mydata[currentLogLocation].ProcessList.OrderByDescending(p => p.Cpu));
            UpdateColumnHeaders(log.mydata[currentLogLocation]);
            procListComboBox = new ObservableCollection<LogProcessEntry>(log.mydata[currentLogLocation].ProcessList.OrderByDescending(p => p.Cpu));
            procListComboBox.Insert(0, system);
        }

        private void Back()
        {
            currentLogLocation--;
            DecProgressBar();
            liveGraph.BackOne();
            LogProcList = new ObservableCollection<LogProcessEntry>(log.mydata[currentLogLocation].ProcessList.OrderByDescending(p => p.Cpu));
            UpdateColumnHeaders(log.mydata[currentLogLocation]);
            procListComboBox = new ObservableCollection<LogProcessEntry>(log.mydata[currentLogLocation].ProcessList.OrderByDescending(p => p.Cpu));
            procListComboBox.Insert(0, system);

            if (currentLogLocation < 0)
            {
                currentLogLocation = -1;
            }
        }

        private void IncProgressBar()
        {
            this.Dispatcher.Invoke(() =>
            {
                pBar.Value = (int) currentLogLocation;
            });
        }

        private void DecProgressBar()
        {
            this.Dispatcher.Invoke(() =>
            {
                pBar.Value = (int) currentLogLocation;
            });
        }

        private void SetProgressBarMax(int max)
        {
            pBar.Dispatcher.Invoke(() => pBar.Maximum = max, DispatcherPriority.Background);
        }

        private void NotifyLocationHasChanged()
        {
            if (currentLogLocation >= maxLogLocation - 2)
            {
                this.Dispatcher.Invoke(() =>
                {

                    paused = true;
                    pauseEvent.Reset();
                    PauseButton.Content = "Resume";
                    PauseButton.IsEnabled = false;
                    StepForward.IsEnabled = true;
                    StepBack.IsEnabled = true;
                });
            }
            else
            {
                if (paused)
                {
                    this.Dispatcher.Invoke(() => {
                        PauseButton.Content = "Resume";
                        PauseButton.IsEnabled = true;
                        StepForward.IsEnabled = true;
                        StepBack.IsEnabled = true;
                    });
                }
                else
                {
                    this.Dispatcher.Invoke(() => {
                        PauseButton.Content = "Pause";
                        PauseButton.IsEnabled = true;
                        StepForward.IsEnabled = false;
                        StepBack.IsEnabled = false;
                    });
                }
            }
        }

        #endregion

        #region Helpers
        private void ResetUI()
        {
            liveGraph.Clear();
            LogProcList = new ObservableCollection<LogProcessEntry>();
            ResetColumnHeaders();
            pBar.Value = 0;
        }

        public void ResetColumnHeaders()
        {
            this.Dispatcher.Invoke(() =>
            {
                listView_gridView.Columns[0].Header = $"Process";
                listView_gridView.Columns[1].Header = $"CPU";
                listView_gridView.Columns[2].Header = $"GPU";
                listView_gridView.Columns[3].Header = $"Memory";
                listView_gridView.Columns[4].Header = $"Disk";
                listView_gridView.Columns[5].Header = $"Network";
            });
        }

        public void UpdateColumnHeaders(data load)
        {
            this.Dispatcher.Invoke(() =>
            {
                listView_gridView.Columns[0].Header = $"Process {load.ProcessList.Count}";
                listView_gridView.Columns[1].Header = $"CPU {Math.Round(load.Cpu, 2)}%";
                listView_gridView.Columns[2].Header = $"GPU {Math.Round(load.Gpu, 2)}%";
                listView_gridView.Columns[3].Header = $"Memory {Math.Round(load.Memory, 2)}%";
                listView_gridView.Columns[4].Header = $"Disk {Math.Round(load.Disk, 2)}%";
                listView_gridView.Columns[5].Header = $"Network {Math.Round(load.Network, 2)}%";

                if (Math.Round(load.Cpu, 2) > 100)
                {
                    listView_gridView.Columns[1].Header = $"CPU 100%";
                }
            });
        }

        public void GetLogList()
        {
            string[] files = Directory.GetFiles(Globals._log.logPath);
            List<LogDetails> tempLogList = new List<LogDetails>();
            for (int i = 0; i < files.Length; i++)
            {
                string[] split = files[i].Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);
                tempLogList.Add(new LogDetails()
                {
                    name = split[split.Length - 1],
                    path = files[i]
                });
            }

            LogList = new ObservableCollection<LogDetails>(tempLogList);
        }



        #endregion

        #region UI Events

        private void Delete_Click(object sender, EventArgs e)
        {
            LogDetails logDeets = (LogDetails)logList.Items[logList.SelectedIndex];
            int ret = Globals._log.DeleteIt(logDeets.name);
            if (ret == 0)
            {
                GetLogList();
            }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboBox1.SelectedItem != null)
            {
                LogProcessEntry selected = (LogProcessEntry)comboBox1.SelectedItem;

                if (selected.Name != null)
                {
                    liveGraph.ProcessPid = selected.Pid;
                }
                else
                {
                    liveGraph.ProcessPid = -1;
                }
            }
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
            var checkBox = sender as System.Windows.Controls.CheckBox;
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

        private void DeleteLog_Click(object sender, RoutedEventArgs e)
        {
            // TODO
        }

        private void ComboBox_Graph_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void logList_Click(object sender, RoutedEventArgs e)
        {
            if (logList.SelectedIndex > -1)
            {
                SelectedLog = (LogDetails)logList.Items[logList.SelectedIndex];
            }
        }

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public ObservableCollection<LogDetails> LogList
        {
            get { return _logList; }
            set
            {
                _logList = value;
                OnPropertyChanged(nameof(LogList));
            }
        }

        public LogDetails SelectedLog
        {
            get { return _selectedLog; }
            set
            {
                _selectedLog = value;
                OnPropertyChanged(nameof(SelectedLog));
            }
        }

        public ObservableCollection<LogProcessEntry> LogProcList
        {
            get { return _logProcList; }
            set
            {
                _logProcList = value;
                OnPropertyChanged(nameof(LogProcList));
            }
        }

        public ObservableCollection<LogProcessEntry> procListComboBox
        {
            get { return _procListComboBox; }
            set
            {
                _procListComboBox = value;
                OnPropertyChanged(nameof(procListComboBox));
            }
        }

        public LogProcessEntry selectedProcessComboBox
        {
            get { return _selectedProcessComboBox; }
            set
            {
                _selectedProcessComboBox = value;
                OnPropertyChanged(nameof(selectedProcessComboBox));
            }
        }

        #endregion
    }
}

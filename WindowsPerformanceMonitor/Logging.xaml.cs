using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WindowsPerformanceMonitor.Graphs;
using WindowsPerformanceMonitor.Models;
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
            Globals._logRef = this;
        }

        private void OnControlLoaded(object sender, RoutedEventArgs e)
        {
            mainWindow = Window.GetWindow(this) as MainWindow;
        }

        private void InitializeComboBox()
        {
            
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
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
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

        private void PlayLog_Click(object sender, RoutedEventArgs e)
        {
            ResetUI();
            currentLogLocation = -1;
            if (SelectedLog != null)
            {

                if (readThread.IsAlive)
                {
                    readThread.Abort();
                }

                paused = false;
                PauseButton.Content = "Pause";
                readThread = new Thread(() =>
                {
                    Play(SelectedLog.path);
                });
                readThread.Start();
            }
        }

        private void ResetUI()
        {
            liveGraph.Clear();
            LogProcList = new ObservableCollection<LogProcessEntry>();
            ResetColumnHeaders();
        }

        private void PauseLog_Click(object sender, RoutedEventArgs e)
        {
            if (readThread != null)
            {
                if(!paused)
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

        private void Play(string path)
        {
            payload log = Globals._log.ReadIt(path);
            liveGraph.connect(log);
            maxLogLocation = log.mytimes.Count - 1;

            while (currentLogLocation < log.mytimes.Count - 1)
            {
                pauseEvent.WaitOne(Timeout.Infinite);
                if (back == true)
                {
                    /**
                     * Step backwards
                     */
                    currentLogLocation--;
                    if (currentLogLocation >= 0)
                    {
                        liveGraph.BackOne();
                        LogProcList = new ObservableCollection<LogProcessEntry>(log.mydata[currentLogLocation].ProcessList.OrderByDescending(p => p.Cpu));
                        UpdateColumnHeaders(log.mydata[currentLogLocation]);
                        procListComboBox = new ObservableCollection<LogProcessEntry>(log.mydata[currentLogLocation].ProcessList.OrderByDescending(p => p.Cpu));
                        procListComboBox.Insert(0, system);
                    }

                    /**
                     * If the log reached the end when stepping back,
                     * reset to zero so if they resume we don't crash.
                     */
                    if (currentLogLocation < 0)
                    {
                        currentLogLocation = -1;
                    }

                    back = false;

                }
                else
                {
                    /**
                     * Step forward or play the log.
                     */
                    currentLogLocation++;
                    liveGraph.Read(log, currentLogLocation);
                    LogProcList = new ObservableCollection<LogProcessEntry>(log.mydata[currentLogLocation].ProcessList.OrderByDescending(p => p.Cpu));
                    UpdateColumnHeaders(log.mydata[currentLogLocation]);
                    procListComboBox = new ObservableCollection<LogProcessEntry>(log.mydata[currentLogLocation].ProcessList.OrderByDescending(p => p.Cpu));
                    procListComboBox.Insert(0, system);
                }

                if (paused == true)
                {
                    /**
                     * If this is true, we know we were stepping through the log.
                     * Reset the pauseEvent so we don't keep reading. We also no longer
                     * need to sleep since we know we're keeping it paused.
                     */
                    pauseEvent.Reset();
                }
                else
                {
                    // TODO: Make this dynamic so we can change speed.
                    Thread.Sleep(2000);
                }
            }

            App.Current.Dispatcher.Invoke(() => {
                paused = true;
                pauseEvent.Reset();
                PauseButton.Content = "Resume";
                StepForward.IsEnabled = true;
                StepBack.IsEnabled = true;
            });
        }

        private void Step_Forward(object sender, RoutedEventArgs e)
        {
            pauseEvent.Set();
        }

        private void Step_Back(object sender, RoutedEventArgs e)
        {
            back = true;
            pauseEvent.Set();
        }

        private void StartLog_Click(object sender, RoutedEventArgs e)
        {
            Globals._log.StartLog();
        }

        private void StopLog_Click(object sender, RoutedEventArgs e)
        {
            if (Globals._log != null)
            {
                Globals._log.WriteIt();
                GetLogList();
            }

            Globals._log = new Log();
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

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static WindowsPerformanceMonitor.Log;

namespace WindowsPerformanceMonitor
{
    /// <summary>
    /// Interaction logic for CompareLogs.xaml
    /// </summary>
    public partial class CompareLogs : Window
    {
        public Window mainWindowRef = null;
        private ObservableCollection<LogDetails> _logList { get; set; }
        private LogDetails _selectedLog1 { get; set; }
        private LogDetails _selectedLog2 { get; set; }
        private ObservableCollection<LogProcessEntry> _logProcList { get; set; }

        private Thread readThread;
        public payload log;
        public int currentLogLocation = -1;
        public int maxLogLocation = 0;
        ManualResetEvent pauseEvent = new ManualResetEvent(true);

        public CompareLogs(Window window)
        {
            InitializeComponent();
            this.DataContext = this;
            LogList = new ObservableCollection<LogDetails>();
            GetLogList();
            readThread = new Thread(() => { });

            mainWindowRef = window;
            this.Closed += new EventHandler(Window_Closed);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public class LogDetails
        {
            public string name { get; set; }
            public string path { get; set; }
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

        void Window_Closed(object sender, EventArgs e)
        {
            mainWindowRef.Show();
        }

        #region UI Handling

        private void logList1_Click(object sender, RoutedEventArgs e)
        {
            if (logList1.SelectedIndex > -1)
            {
                SelectedLog1 = (LogDetails)logList1.Items[logList1.SelectedIndex];
            }
        }

        private void logList2_Click(object sender, RoutedEventArgs e)
        {
            if (logList2.SelectedIndex > -1)
            {
                SelectedLog2 = (LogDetails)logList2.Items[logList2.SelectedIndex];
            }
        }

        private void CompareBtn_Click(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show("1: " + SelectedLog1.name + ", " + SelectedLog1.path + "\n2: " + SelectedLog2.name + ", " + SelectedLog2.path);

            ResetUI();
            currentLogLocation = -1;
            if (SelectedLog1 != null)
            {
                readThread.Abort();

                readThread = new Thread(() =>
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        //paused = false;
                        pauseEvent.Set();
                        /*PauseButton.Content = "Pause";
                        StepForward.IsEnabled = false;
                        StepBack.IsEnabled = false;*/
                    });

                    currentLogLocation = -1;
                    ConnectLog(SelectedLog1.path);
                    Play(SelectedLog1.path);
                });

                readThread.Start();
            }
        }

        #endregion

        private void ConnectLog(string path)
        {
            log = Globals._log.ReadIt(path);
            liveGraph1.connect();
            maxLogLocation = log.mytimes.Count - 1;
            //SetProgressBarMax(maxLogLocation - 2);
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
                Thread.Sleep((int)(100 * 0.5));
            }
        }

        private void Forward()
        {
            currentLogLocation++;
            Console.WriteLine("CURRENT LOG LOCATION: " + currentLogLocation);
            //IncProgressBar();
            liveGraph1.Read(log, currentLogLocation);
            LogProcList = new ObservableCollection<LogProcessEntry>(log.mydata[currentLogLocation].ProcessList.OrderByDescending(p => p.Cpu));
            //UpdateColumnHeaders(log.mydata[currentLogLocation]);
            //procListComboBox = new ObservableCollection<LogProcessEntry>(log.mydata[currentLogLocation].ProcessList.OrderByDescending(p => p.Cpu));
            //procListComboBox.Insert(0, system);
        }

        private void NotifyLocationHasChanged()
        {
            if (currentLogLocation >= maxLogLocation - 2)
            {
       
                this.Dispatcher.Invoke(() =>
                {
                    //paused = true;
                    pauseEvent.Reset();
                    MessageBox.Show("END");
                    /*PauseButton.Content = "Resume";
                    PauseButton.IsEnabled = false;
                    StepForward.IsEnabled = true;
                    StepBack.IsEnabled = true;*/
                });
                
            }
            /*else
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
            }*/
        }

        private void ResetUI()
        {
            liveGraph1.Clear();
            LogProcList = new ObservableCollection<LogProcessEntry>();
            //ResetColumnHeaders();
            //pBar.Value = 0;
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

        public LogDetails SelectedLog1
        {
            get { return _selectedLog1; }
            set
            {
                _selectedLog1 = value;
                OnPropertyChanged(nameof(SelectedLog1));
            }
        }

        public LogDetails SelectedLog2
        {
            get { return _selectedLog2; }
            set
            {
                _selectedLog2 = value;
                OnPropertyChanged(nameof(SelectedLog2));
            }
        }

    }
}

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

        private Thread readThread1;
        private Thread readThread2;
        public payload log1;
        public payload log2;
        public int currentLogLocation1 = -1;
        public int currentLogLocation2 = -1;
        public int maxLogLocation1 = 0;
        public int maxLogLocation2 = 0;
        ManualResetEvent pauseCompareEvent1 = new ManualResetEvent(true);
        ManualResetEvent pauseCompareEvent2 = new ManualResetEvent(true);

        public CompareLogs(Window window)
        {
            InitializeComponent();
            this.DataContext = this;
            LogList = new ObservableCollection<LogDetails>();
            GetLogList();
            readThread1 = new Thread(() => { });
            readThread2 = new Thread(() => { });

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
            ResetUI();
            currentLogLocation1 = -1;
            currentLogLocation2 = -1;
            if (SelectedLog1 != null && SelectedLog2 != null)
            {
                readThread1.Abort();
                readThread2.Abort();

                readThread1 = new Thread(() =>
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        pauseCompareEvent1.Set();
                    });

                    currentLogLocation1 = -1;
                    ConnectLog(SelectedLog1.path, 1);
                    Play(SelectedLog1.path, 1);
                });

                readThread2 = new Thread(() =>
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        pauseCompareEvent2.Set();
                    });

                    currentLogLocation2 = -1;
                    ConnectLog(SelectedLog2.path, 2);
                    Play(SelectedLog2.path, 2);
                });

                readThread1.Start();
                readThread2.Start();
            }
        }

        #endregion

        private void ResetUI()
        {
            liveGraph1.Clear();
            liveGraph2.Clear();
        }

        private void ConnectLog(string path, int logNum)
        {
            if (logNum == 1)
            {
                log1 = Globals._log.ReadIt(path);

                liveGraph1.connect(log1);
                maxLogLocation1 = log1.mytimes.Count - 1;
            } else
            {
                // logNum == 2
                log2 = Globals._log.ReadIt(path);

                liveGraph2.connect(log2);
                maxLogLocation2 = log2.mytimes.Count - 1;
            }
        }

        private void Play(string path, int logNum)
        {
            while (true)
            {
                if (logNum == 1)
                {
                    pauseCompareEvent1.WaitOne(Timeout.Infinite);

                    if (currentLogLocation1 + 2 > maxLogLocation1)
                    {
                        continue;
                    }

                    Forward(1);
                    NotifyLocationHasChanged(1);
                } else
                {
                    pauseCompareEvent2.WaitOne(Timeout.Infinite);

                    if (currentLogLocation2 + 2 > maxLogLocation2)
                    {
                        continue;
                    }

                    Forward(2);
                    NotifyLocationHasChanged(2);
                }

                Thread.Sleep((int)(100 * 0.5));
            }
        }

        private void Forward(int logNum)
        {
            if (logNum == 1)
            {
                currentLogLocation1++;
                liveGraph1.Read(log1, currentLogLocation1);
            } else
            {
                currentLogLocation2++;
                liveGraph2.Read(log2, currentLogLocation2);
            }
        }

        private void NotifyLocationHasChanged(int logNum)
        {
            if (logNum == 1)
            {
                if (currentLogLocation1 >= maxLogLocation1 - 2)
                {

                    this.Dispatcher.Invoke(() =>
                    {
                        pauseCompareEvent1.Reset();
                    });

                }
            }
            else
            {
                if (currentLogLocation2 >= maxLogLocation2 - 2)
                {

                    this.Dispatcher.Invoke(() =>
                    {
                        pauseCompareEvent2.Reset();
                    });

                }
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

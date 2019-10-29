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
using System.Windows.Navigation;
using System.Windows.Shapes;
using WindowsPerformanceMonitor.Models;
using static WindowsPerformanceMonitor.Log;

namespace WindowsPerformanceMonitor
{
    /// <summary>
    /// Interaction logic for Logging.xaml
    /// </summary>
    /// 

    public partial class Logging : UserControl, INotifyPropertyChanged
    {
        private MainWindow mainWindow = null;
        private ObservableCollection<LogDetails> _logList { get; set; }
        private ObservableCollection<ProcessEntry> _logProcList { get; set; }
        private LogDetails _selectedLog { get; set; }

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
        }

        private void OnControlLoaded(object sender, RoutedEventArgs e)
        {
            mainWindow = Window.GetWindow(this) as MainWindow;
        }

        private void InitializeComboBox()
        {
            
        }

        private void GetLogList()
        {
            string[] files = Directory.GetFiles("C:\\Users\\Darren\\Documents\\WindowsPerformanceMonitor");
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
            if (SelectedLog != null)
            {
                Task.Run(() =>
                {
                    Play(SelectedLog.path);
                });
            }
        }

        private void Play(string path)
        {
            payload log = Globals._log.ReadIt(path);
            for (int i = 0; i < log.mytimes.Count; i++)
            {
                LogProcList = new ObservableCollection<ProcessEntry>(log.mydata[i].ProcessList.OrderByDescending(p => p.Cpu));
                UpdateColumnHeaders(log.mydata[i]);
                Thread.Sleep(2000);
            }

            // This clears the listview after log has finished reading.
            LogProcList = new ObservableCollection<ProcessEntry>();
            ResetColumnHeaders();
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

        private void PauseLog_Click(object sender, RoutedEventArgs e)
        {
            // TODO:
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

        public ObservableCollection<ProcessEntry> LogProcList
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

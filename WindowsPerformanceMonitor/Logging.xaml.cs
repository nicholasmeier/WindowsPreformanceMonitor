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
        Log temp;
        private MainWindow mainWindow = null;
        public ObservableCollection<logNames> _logs { get; set; }
        public ObservableCollection<ProcessEntry> _logProcList { get; set; }
        public logNames _selectedLog { get; set; }

        public class logNames
        {
            public string name { get; set; }
            public string path { get; set; }
        }

        public Logging()
        {
            InitializeComponent();
            temp = new Log();
            Logs = new ObservableCollection<logNames>();
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
            List<logNames> tempLogList = new List<logNames>();
            for (int i = 0; i < files.Length; i++)
            {
                string[] split = files[i].Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);
                tempLogList.Add(new logNames()
                {
                    name = split[split.Length - 1],
                    path = files[i]
                });
            }

            Logs = new ObservableCollection<logNames>(tempLogList);
            Console.WriteLine(Logs);
        }

        private void StartLog_Click(object sender, RoutedEventArgs e)
        {
            temp.StartLog();
        }

        private void StopLog_Click(object sender, RoutedEventArgs e)
        {
            if (temp != null)
            {
                temp.WriteIt();
                GetLogList();
            }

            temp = null;
        }

        private void PlayLog_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedLog != null)
            {
                payload log = temp.ReadIt(SelectedLog.path);
                // Async so we can do other stuff while its reading.
                Task.Run(() =>
                {
                    Play(log);
                });
            }
        }

        private void Play(payload log)
        {
            for (int i = 0; i < log.mytimes.Count; i++)
            {
                LogProcList = new ObservableCollection<ProcessEntry>(log.mydata[i].ProcessList.OrderByDescending(p => p.Cpu));
                Thread.Sleep(2000);
            }

            // This clears the listview after log has finished reading.
            LogProcList = new ObservableCollection<ProcessEntry>();
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
                SelectedLog = (logNames)logList.Items[logList.SelectedIndex];
            }
        }

        private void testingRead()
        {
            payload log = temp.ReadIt("C:\\Users\\Darren\\Documents\\WindowsPerformanceMonitor\\10-27-2019");

        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public ObservableCollection<logNames> Logs
        {
            get { return _logs; }
            set
            {
                _logs = value;
                OnPropertyChanged(nameof(Logs));
            }
        }

        public logNames SelectedLog
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;
using static WindowsPerformanceMonitor.Log;
using Newtonsoft.Json;

namespace WindowsPerformanceMonitor
{
    /// <summary>
    /// Interaction logic for LogReport.xaml
    /// </summary>
    public partial class LogReport : Window
    {
        public payload log;
        public String avgCpu;
        public double avgGpu;
        public double avgMem;
        public double avgDisk;
        public double avgNetwork;
        public double avgCpuTemp;
        public double avgGpuTemp;
        public averageData data;

        public struct averageData
        {
            public double avgCpu;
            public double avgGpu;
            public double avgMemory;
            public double avgDisk;
            public double avgNetwork;
            public double avgCpuTemp;
            public double avgGpuTemp;
        }

        public LogReport(String path, String logName)
        {
            InitializeComponent();
            DataContext = this;
            data = new averageData();

            log = Globals._log.ReadIt(path);
            
            int len = log.mydata.Count;
            double totalCpu = 0;
            double totalGpu = 0;
            double totalMem = 0;
            double totalDisk = 0;
            double totalNetwork = 0;
            double totalCpuTemp = 0;
            double totalGpuTemp = 0;

            //MessageBox.Show(path);

            DateTime start = log.mystart;
            DateTime end = log.mytimes[len - 1];

            titleLabel.Content = logName+" Log Report";
            dateLabel.Content = "(" + start.ToString() + ") to (" + end.ToString() + ")";

            for (int i = 0; i < len; i++)
            {
                totalCpu += log.mydata[0].Cpu;
                totalGpu += log.mydata[0].Gpu;
                totalMem += log.mydata[0].Memory;
                totalDisk += log.mydata[0].Disk;
                totalNetwork += log.mydata[0].Network;
                //if (log.mydata[0].CpuTemp.is)
                totalCpuTemp += log.mydata[0].CpuTemp;
                totalGpuTemp += log.mydata[0].GpuTemp;

            }

            data.avgCpuTemp = (totalCpu / len);
            data.avgGpuTemp = (totalGpu / len);
            data.avgCpu = (totalMem / len);
            data.avgGpu = (totalDisk / len);
            data.avgMemory = (totalNetwork / len);
            data.avgNetwork = (totalCpuTemp / len);
            data.avgDisk = (totalGpuTemp / len);

            avgCpuLabel.Content = "Average CPU Usage: " + (totalCpu / len).ToString();
            avgGpuLabel.Content = "Average GPU Usage: " + (totalGpu / len).ToString();
            avgMemLabel.Content = "Average Memory Usage: " + (totalMem / len).ToString();
            avgDiskLabel.Content = "Average Disk Usage: " + (totalDisk / len).ToString();
            avgNetworkLabel.Content = "Average Network Usage: " + (totalNetwork / len).ToString();
            avgCpuTempLabel.Content = "Average CPU Temperature: " + (totalCpuTemp / len).ToString();
            avgGpuTempLabel.Content = "Average GPU Temperature: " + (totalGpuTemp / len).ToString();

        }

        private void SaveLogReport_Click(object sender, RoutedEventArgs e)
        {
            String json = JsonConvert.SerializeObject(data);
            String fileName = log.mystart.Date.Month.ToString() + "-" + log.mystart.Date.Day.ToString() + "-" + log.mystart.Date.Year.ToString()+"_Report";
            String logPath = Path.Combine(Environment.ExpandEnvironmentVariables("%userprofile%"), "Documents", "WindowsPerformanceMonitor");

            if (!System.IO.Directory.Exists(Path.Combine(logPath)))
            {
                System.IO.Directory.CreateDirectory(Path.Combine(logPath));
            }

            int i = 0;
            string appendage = "";

            while (File.Exists(Path.Combine(logPath, fileName + appendage + ".txt")))
            {
                i++;
                appendage = "(" + i + ")";
            }

            System.IO.File.WriteAllText(Path.Combine(logPath, fileName + appendage + ".txt"), json);
        }

    }
}

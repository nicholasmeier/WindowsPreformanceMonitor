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
        public String logNamee; // for log report name
        public DateTime realTimeName; // for real time report name 
        
        public averageData data;
        public int reportType;  // log report -> 0, real time report -> 1

        public struct averageData
        {
            public DateTime startDate;
            public DateTime endDate;
            public double avgCpu;
            public double avgGpu;
            public double avgMemory;
            public double avgDisk;
            //public double avgNetwork;
            public double avgIO;
            public double avgCpuTemp;
            public double avgGpuTemp;
        }

        public LogReport(DateTime currTime, double avCpu, double avGpu, double avMemory, double avDisk, double avIO)
        {
            avCpu = Math.Round(avCpu, 3);
            avGpu = Math.Round(avGpu, 3);
            avMemory = Math.Round(avMemory, 3);
            avDisk = Math.Round(avDisk, 3);
            avIO = Math.Round(avIO, 3);

            reportType = 1;
            InitializeComponent();
            DataContext = this;
            data = new averageData();

            data.avgCpu = avCpu;
            data.avgGpu = avGpu;
            data.avgMemory = avMemory;
            data.avgDisk = avDisk;
            data.avgIO = avIO;

            realTimeName = currTime;
            data.startDate = realTimeName;

            titleLabel.Content = "Real Time Report";
            dateLabel.Content = "(" + currTime.ToString() + ")";
            avgCpuLabel.Content = "Average CPU Usage: " + avCpu.ToString() + " %";
            avgGpuLabel.Content = "Average GPU Usage: " + avGpu.ToString() + " %";
            avgMemLabel.Content = "Average Memory Usage: " + avMemory.ToString() + " MB";
            avgDiskLabel.Content = "Average Disk Usage: " + avDisk.ToString() + " Mb/s";
            avgIOLabel.Content = "Average IO Usage: " + avIO.ToString() + " Mb/s";
        }

        public LogReport(String path, String logName)
        {
            reportType = 0;
            InitializeComponent();
            DataContext = this;
            data = new averageData();

            log = Globals._log.ReadIt(path);
            
            int len = log.mydata.Count;
            double totalCpu = 0;
            double totalGpu = 0;
            double totalMem = 0;
            double totalDisk = 0;
            //double totalNetwork = 0;
            double totalCpuTemp = 0;
            double totalGpuTemp = 0;

            DateTime start = log.mystart;
            DateTime end = log.mytimes[len - 1];
            data.startDate = start;
            data.endDate = end;

            logNamee = logName;
            titleLabel.Content = logName+" Log Report";
            dateLabel.Content = "(" + start.ToString() + ") to (" + end.ToString() + ")";

            for (int i = 0; i < len; i++)
            {
                totalCpu += log.mydata[0].Cpu;
                totalGpu += log.mydata[0].Gpu;
                totalMem += log.mydata[0].Memory;
                totalDisk += log.mydata[0].Disk;
                //totalNetwork += log.mydata[0].Network;
                totalCpuTemp += log.mydata[0].CpuTemp;
                totalGpuTemp += log.mydata[0].GpuTemp;

            }

            data.avgCpu = (totalCpu / len);
            data.avgGpu = (totalGpu / len);
            data.avgMemory = (totalMem / len);
            data.avgDisk = (totalDisk / len);
            //data.avgNetwork = (totalNetwork / len);
            data.avgCpuTemp = (totalCpuTemp / len);
            data.avgGpuTemp = (totalGpuTemp / len);

            avgCpuLabel.Content = "Average CPU Usage: " + (totalCpu / len).ToString() + " %";
            avgGpuLabel.Content = "Average GPU Usage: " + (totalGpu / len).ToString() + " %";
            avgMemLabel.Content = "Average Memory Usage: " + (totalMem / len).ToString() + " MB";
            avgDiskLabel.Content = "Average Disk Usage: " + (totalDisk / len).ToString() + " Mb/s";
            //avgNetworkLabel.Content = "Average Network Usage: " + (totalNetwork / len).ToString();
            avgIOLabel.Content = "Average IO Usage: " + "0 Mb/s";
            avgCpuTempLabel.Content = "Average CPU Temperature: " + (totalCpuTemp / len).ToString();
            avgGpuTempLabel.Content = "Average GPU Temperature: " + (totalGpuTemp / len).ToString();

        }

        private void SaveLogReport_Click(object sender, RoutedEventArgs e)
        {
            String json = JsonConvert.SerializeObject(data);

            String fileName = "";
            String logPath = "";

            if (reportType == 0)
            {
                fileName = logNamee + "_LogReport";
                logPath = Path.Combine(Environment.ExpandEnvironmentVariables("%userprofile%"), "Documents", "WindowsPerformanceMonitorLogReports");
            } else
            {
                fileName = realTimeName.Date.Month.ToString() + "-" + realTimeName.Date.Day.ToString() + "-" + realTimeName.Date.Year.ToString() + "_RealTimeReport";
                logPath = Path.Combine(Environment.ExpandEnvironmentVariables("%userprofile%"), "Documents", "WindowsPerformanceMonitorRealTimeReports");
            }

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

            MessageBox.Show("Report Successfully Saved");
            this.Hide();
        }

    }
}

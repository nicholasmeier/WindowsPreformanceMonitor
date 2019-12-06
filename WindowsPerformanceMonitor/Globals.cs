using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenHardwareMonitor.Hardware;

namespace WindowsPerformanceMonitor
{
    public static class Globals
    {
        public static Log _log { get; set; }
        public static string _logFileType = ".txt";
        public static bool _encryptionEnabled = false;
        public static ComputerStatsMonitor _provider;
        public static Models.ProcessEntry OverlayProc;
        public static Logging _logRef { get; set; }
        public static UserSettings Settings { get; set; }
        public static ComputerStatsMonitor provider
        {
            get { return _provider; }
        }
        public static void SetProvider(ComputerStatsMonitor csm)
        {
            _provider = csm;
        }
        public static bool limit = true;
        public static double totCpu = 0.0;
        public static double totGpu = 0.0;
        public static double totMem = 0.0;
        public static double totCpuTemp = 0.0;
        public static double totGpuTemp = 0.0;
        public static double totDisk = 0.0;
        public static double totIO = 0.0;
        public static long len = 0;
    }
}

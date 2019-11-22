using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Media;
using WindowsPerformanceMonitor.Backend;

namespace WindowsPerformanceMonitor.Models
{
    public class ProcessEntry
    {
        public Process Proc { get; set; }
        public string Name { get; set; }
        public int Pid { get; set; }
        public double Cpu { get; set; }
        public Tuple<DateTime, TimeSpan> PrevCpu { get; set; }
        public double Gpu { get; set; }
        public double Memory { get; set; }
        public float Disk { get; set; }
        public float PrevDisk { get; set; }
        public float PrevNetwork { get; set; }
        public DateTime PrevTime { get; set; }
        public float Network { get; set; }
        public bool IsApplication { get; set; }
        public string ApplicationName { get; set; }
        public string ApplicationPath { get; set; }
        public int Ppid { get; set; }
        public List<ProcessEntry> ChildProcesses { get; set; }
        public int parentPid { get; set; }
        // These are for notifications
        public double cpuThreshold;
        public DateTime cpuThresholdPassedTime;

        public double gpuThreshold;
        public DateTime gpuThresholdPassedTime;

        public double memoryThreshold;
        public DateTime memoryThresholdPassedTime;
        public string LogScheduleTime { get; set; }
        public string LogScheduleDuration { get; set; }
        public NetworkInfo networkInfo { get; set; }
        public Icon Icon { get; set; }

        public ImageSource IE { get; set; }
    }
}

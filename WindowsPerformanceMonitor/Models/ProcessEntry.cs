using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;

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
        public int Ppid { get; set; }
        public List<ProcessEntry> ChildProcesses { get; set; }
        public int parentPid { get; set; }
        // These are for notifications
        public double CpuThreshold { get; set; }
        public double GpuThreshold { get; set; }
        public double MemoryThreshold { get; set; }
        public string LogScheduleTime { get; set; }
        public string LogScheduleDuration { get; set; }
    }
}

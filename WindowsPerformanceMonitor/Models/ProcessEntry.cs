using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;

namespace WindowsPerformanceMonitor.Models
{
    public class ProcessEntry
    {
        public string Name { get; set; }
        public int Pid { get; set; }
        public double Cpu { get; set; }
        public Tuple<DateTime, TimeSpan> PrevCpu { get; set; }
        public double Gpu { get; set; }
        public Tuple<DateTime, TimeSpan> PrevGpu { get; set; }
        public double Memory { get; set; }
        public Tuple<DateTime, TimeSpan> PrevMemory { get; set; }
        public double Disk { get; set; }
        public Tuple<DateTime, TimeSpan> PrevDisk { get; set; }
        public double Network { get; set; }
        public Tuple<DateTime, TimeSpan> PrevNetwork { get; set; }
        public bool IsApplication { get; set; }
        public string ApplicationName { get; set; }
        public int Ppid { get; set; }
        public List<ProcessEntry> ChildProcesses { get; set; }
        public int parentPid { get; set; }
    }
}

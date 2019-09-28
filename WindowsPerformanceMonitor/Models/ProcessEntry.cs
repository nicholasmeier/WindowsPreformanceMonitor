using System;
using System.ComponentModel;
using System.Diagnostics;

namespace WindowsPerformanceMonitor.Models
{
    public class ProcessEntry
    {
        public string Name { get; set; }
        public int Pid { get; set; }
        public double Cpu { get; set; }
        public double Gpu { get; set; }
        public double Memory { get; set; }
        public double Disk { get; set; }
        public double Network { get; set; }
    }
}

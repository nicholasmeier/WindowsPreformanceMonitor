using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsPerformanceMonitor.Models
{
    class Trend
    {
        public double Cpu { get; set; }
        public double Gpu { get; set; }
        public double Memory { get; set; }
        public double Disk { get; set; }
        public double Network { get; set; }
        public double CpuTemp { get; set; }
        public double GpuTemp { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsPerformanceMonitor.Models
{
    class CPUHardwareDetails
    {
        public string _name { get; set; }
        public double _busSpeedCPU { get; set; }
        public double _clockSpeedCPU { get; set; }
        public int _coresCPU { get; set; }
        public int _logicalCoresCPU { get; set; }
        public Boolean _virtualizationCPU { get; set; }
        public List<uint> _cacheSizesCPU { get; set; }
    }
}

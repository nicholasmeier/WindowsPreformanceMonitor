using OpenHardwareMonitor.Hardware;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsPerformanceMonitor.Models
{
    public class ComputerObj
    {
        public Computer Computer { get; set; }
        public ObservableCollection<ProcessEntry> ProcessList { get; set; } 
    }
}

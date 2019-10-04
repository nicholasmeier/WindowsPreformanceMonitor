using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;

namespace WindowsPerformanceMonitor.Models
{
    public class ApplicationStartup
    {
        public string Name { get; set; }
        public string Publisher { get; set; }
        public string Status { get; set; }
    }
}
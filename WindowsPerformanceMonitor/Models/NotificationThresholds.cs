using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsPerformanceMonitor.Models
{
    class NotificationThresholds
    {
        public double cpuThreshold;
        public bool cpuThresholdPassed;
        public DateTime cpuThresholdPassedTime;

        public double gpuThreshold;
        public bool gpuThresholdPassed;
        public DateTime gpuThresholdPassedTime;

        public double memoryThreshold;
        public bool memoryThresholdPassed;
        public DateTime memoryThresholdPassedTime;

    }
}

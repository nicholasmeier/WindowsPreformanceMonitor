using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WindowsPerformanceMonitor.Models;

namespace WindowsPerformanceMonitor.Backend
{
    class Processes
    {
        public List<ProcessEntry> GetProcesses()
        {
            List<ProcessEntry> processEntries = new List<ProcessEntry>();

            Process[] processes = Process.GetProcesses();
            for (int i = 0; i < processes.Length; i++)
            {
                ProcessEntry p = new ProcessEntry()
                {
                    Name = processes[i].ProcessName,
                    Pid = processes[i].Id,
                    Cpu = 0,
                    Memory = 0,
                    Gpu = 0,
                    Disk = 0,
                    Network = 0
                };

                processEntries.Add(p);
            }

            return processEntries;
        }

        public double GetCpu(int pid)
        {
            return 0;
       
        }
    }
}

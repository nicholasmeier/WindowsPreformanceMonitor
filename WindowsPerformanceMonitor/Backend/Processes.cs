using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
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
                    Cpu = GetCpu(processes[i].Id),
                    Memory = GetMemory(processes[i].Id),
                    Gpu = GetGpu(processes[i].Id),
                    Disk = GetDisk(processes[i].Id),
                    Network = GetNetwork(processes[i].Id)
                };

                processEntries.Add(p);
            }

            return processEntries;
        } 

        public double GetCpu(int pid)
        {
            return 0;
        }

        public double GetGpu(int pid)
        {
            Process p = Process.GetProcessById(pid);
            return 0;
        }

        public double GetMemory(int pid)
        {
            Process p = Process.GetProcessById(pid);
            return 0;
        }

        public double GetDisk(int pid)
        {
            Process p = Process.GetProcessById(pid);
            return 0;
        }

        public double GetNetwork(int pid)
        {
            Process p = Process.GetProcessById(pid);
            return 0;
        }
    }
}

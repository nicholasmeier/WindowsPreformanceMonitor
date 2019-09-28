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

        private static DateTime lastTime;
        private static TimeSpan lastTotalProcessorTime;
        private static DateTime curTime;
        private static TimeSpan curTotalProcessorTime;

        public double GetCpu(int pid)
        {
            return 0;

/*            bool t = true;
            double CPUUsage = 0;

            // This needs to run async or something because its slow as fuck.
            Process p = null;

            try
            {
                while (t)
                {
                    p = Process.GetProcessById(pid);
                    if (lastTime == null || lastTime == new DateTime())
                    {
                        lastTime = DateTime.Now;
                        lastTotalProcessorTime = p.TotalProcessorTime;

                    }
                    else
                    {
                        curTime = DateTime.Now;
                        curTotalProcessorTime = p.TotalProcessorTime;

                        CPUUsage = (curTotalProcessorTime.TotalMilliseconds - lastTotalProcessorTime.TotalMilliseconds) / curTime.Subtract(lastTime).TotalMilliseconds / Convert.ToDouble(Environment.ProcessorCount);

                        lastTime = curTime;
                        lastTotalProcessorTime = curTotalProcessorTime;
                        t = false;
                    }

                    Thread.Sleep(250);
                }

                Console.WriteLine(p.ProcessName + CPUUsage * 100 );
                return CPUUsage * 100;
            }
            catch (Exception e)
            {
                return 0;
            }*/
           
        }

        public double GetGpu(int pid)
        {
            try
            {
                Process p = Process.GetProcessById(pid);
                return 0;
            }
            catch (Exception e)
            {
                return 0;
            }
        }

        public double GetMemory(int pid)
        {
            try
            {
                Process p = Process.GetProcessById(pid);
                return 0;
            }
            catch (Exception e)
            {
                return 0;
            }
        }

        public double GetDisk(int pid)
        {
            try
            {
                Process p = Process.GetProcessById(pid);
                return 0;
            }
            catch (Exception e)
            {
                return 0;
            }
        }

        public double GetNetwork(int pid)
        {
            try
            {
                Process p = Process.GetProcessById(pid);
                return 0;
            }
            catch (Exception e)
            {
                return 0;
            }
        }
    }
}

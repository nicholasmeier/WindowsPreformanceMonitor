using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WindowsPerformanceMonitor.Models;

namespace WindowsPerformanceMonitor.Backend
{
    class Processes
    {
        /// <summary>
        /// Get list of processes of type ProcessEntry
        /// </summary>
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
                    Network = 0,
                    IsApplication = processes[i].MainWindowHandle != IntPtr.Zero ? true : false
                };

                processEntries.Add(p);
            }

            return processEntries;
        }

        /// <summary>
        /// Update CPU usage for a process list
        /// </summary>
        public double updateCpu(ObservableCollection<ProcessEntry> procList)
        {
            List<DateTime> lastTimes = new List<DateTime>(new DateTime[procList.Count]);
            List<TimeSpan> lastTotalProcessorTime = new List<TimeSpan>(new TimeSpan[procList.Count]);

            /* Get the current time and total process usage
                for each process */
            for (int i = 0; i < procList.Count; i++)
            {
                Process p;
                try
                {
                    p = Process.GetProcessById(procList[i].Pid);
                }
                catch (ArgumentException)    // Process no longer running
                {
                    lastTimes.Insert(i, DateTime.Now);
                    lastTotalProcessorTime.Insert(i, new TimeSpan(0));
                    continue;
                }

                if (lastTimes[i] == null || lastTimes[i] == new DateTime())
                {
                    lastTimes.Insert(i, DateTime.Now);
                    try
                    {
                        lastTotalProcessorTime.Insert(i, p.TotalProcessorTime);
                    }
                    catch (Exception)       // WIN32 access denied
                    {
                        lastTotalProcessorTime.Insert(i, new TimeSpan(0));
                    }
                }
            }

            Thread.Sleep(250);
            double totalCpu = 0;

            /* Get the current time and total process usage
                for each process, calculate cpu usage
                based on previous */
            for (int i = 0; i < procList.Count; i++)
            {
                Process p;
                try
                {
                    p = Process.GetProcessById(procList[i].Pid);
                }
                catch (ArgumentException)    // Process no longer running
                {
                    procList[i].Cpu = -1;
                    continue;
                }

                DateTime currTime = DateTime.Now;
                TimeSpan currTotalProcessorTime;
                try
                {
                    currTotalProcessorTime = p.TotalProcessorTime;
                }
                catch (Exception)            // WIN32 access denied.
                {
                    currTotalProcessorTime = new TimeSpan(0);
                }

                double cpuUsage = (currTotalProcessorTime.TotalMilliseconds - lastTotalProcessorTime[i].TotalMilliseconds) / currTime.Subtract(lastTimes[i]).TotalMilliseconds / Convert.ToDouble(Environment.ProcessorCount);
                procList[i].Cpu = cpuUsage * 100;
                totalCpu += cpuUsage;
            }

            return Math.Round(totalCpu * 100, 2);
        }

        /// <summary>
        /// Update memory usage for a process list
        /// </summary>
        public double updateMem(ObservableCollection<ProcessEntry> procList)
        {
            List<long> memoryUsages = new List<long>(new long[procList.Count]);

            for (int i = 0; i < procList.Count; i++)
            {
                Process p;
                try
                {
                    p = Process.GetProcessById(procList[i].Pid);
                }
                catch (ArgumentException)    // Process no longer running
                {
                    memoryUsages.Insert(i, -1);
                    continue;
                }

                if (memoryUsages[i] != -1)
                {
                    memoryUsages.Insert(i, 0);
                    try
                    {
                        memoryUsages.Insert(i, p.WorkingSet64);
                    }
                    catch (Exception)       // The platform is Windows 98 or Windows Millennium Edition which is not supported
                    {
                        memoryUsages.Insert(i, 0);
                    }
                }
            }

            Thread.Sleep(250);
            ulong totalMem = new Microsoft.VisualBasic.Devices.ComputerInfo().TotalPhysicalMemory;
            ulong totalUsed = 0;
            /* Get the current time and total process usage
                for each process, calculate Mem usage
                based on previous */
            for (int i = 0; i < procList.Count; i++)
            {
                Process p;
                try
                {
                    p = Process.GetProcessById(procList[i].Pid);
                }
                catch (ArgumentException)    // Process no longer running
                {
                    procList[i].Memory = -1;
                    continue;
                }
                if(memoryUsages[i] == 0)
                {
                    procList[i].Memory = 0;
                }
                else if(memoryUsages[i] > 0)
                {
                    procList[i].Memory = ((double)memoryUsages[i] / (double)totalMem) * 100;
                    totalUsed += (ulong)memoryUsages[i];
                }
                else
                {
                    procList[i].Memory = -1;
                }
            }
            return Math.Round(((double)totalUsed / (double)totalMem) * 100, 2);
        }

        public double updateDisk(ObservableCollection<ProcessEntry> procList)
        {
            List<long> diskUsages = new List<long>(new long[procList.Count]);

            for (int i = 0; i < procList.Count; i++)
            {
                ProcessDiagnosticInfo pi; 
                Process p;
                try
                {
                    p = Process.GetProcessById(procList[i].Pid);
                }
                catch (ArgumentException)    // Process no longer running
                {
                    diskUsages.Insert(i, -1);
                    continue;
                }

                if (diskUsages[i] != -1)
                {
                    diskUsages.Insert(i, 0);
                    try
                    {
                        diskUsages.Insert(i, p.WorkingSet64);
                    }
                    catch (Exception)       // The platform is Windows 98 or Windows Millennium Edition which is not supported
                    {
                        diskUsages.Insert(i, 0);
                    }
                }
            }

            Thread.Sleep(250);
            ulong totalDisk = 0;
            foreach (DriveInfo d in DriveInfo.GetDrives())
            {
                totalDisk = totalDisk + (ulong)d.TotalSize;
            }
            ulong totalUsed = 0;
            /* Get the current time and total process usage
                for each process, calculate Mem usage
                based on previous */
            for (int i = 0; i < procList.Count; i++)
            {
                Process p;
                try
                {
                    p = Process.GetProcessById(procList[i].Pid);
                }
                catch (ArgumentException)    // Process no longer running
                {
                    procList[i].Disk = -1;
                    continue;
                }
                if (diskUsages[i] == 0)
                {
                    procList[i].Disk = 0;
                }
                else if (diskUsages[i] > 0)
                {
                    procList[i].Disk = ((double)diskUsages[i] / (double)totalDisk) * 100;
                    totalUsed += (ulong)diskUsages[i];
                }
                else
                {
                    procList[i].Disk = -1;
                }
            }
            return Math.Round(((double)totalUsed / (double)totalDisk) * 100, 2);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WindowsPerformanceMonitor.Models;
using System.Runtime.InteropServices;
using System.Management;
using PerformanceMonitor.Cpp.CLI;

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
                uint ppid;
                using (var wrapper = new Logic())
                {
                    ppid = wrapper.getppid(processes[i].Id);
                }
                ProcessEntry p = new ProcessEntry()
                {
                    Name = processes[i].ProcessName,
                    Pid = processes[i].Id,
                    Cpu = 0,
                    Memory = 0,
                    Gpu = 0,
                    Disk = 0,
                    Network = 0,
                    Ppid = ppid,
                    ChildProcesses = GetChildProcesses(processes[i].Id),
                    IsApplication = processes[i].MainWindowHandle != IntPtr.Zero ? true : false
                };
                processEntries.Add(p);
            }

            return processEntries;
        }

        public void Kill(int pid)
        {
            Process p;
            try
            {
                p = Process.GetProcessById(pid);
                p.Kill();
            }
            catch (ArgumentException)
            {
                //
            }
        }
        /// <summary>
        /// Get the child processes for a given process
        /// </summary>
        /// <param name="process"></param>
        public ObservableCollection<ProcessEntry> GetChildProcesses(int pid)
        {
            ObservableCollection<ProcessEntry> childProcesses = new ObservableCollection<ProcessEntry>();
            var results = new List<Process>();
            // query the management system objects for any process that has the current
            // process listed as it's parent
            string queryText = string.Format("select processid from win32_process where parentprocessid = {0}", pid);
            using (var searcher = new ManagementObjectSearcher(queryText))
            {
                foreach (var obj in searcher.Get())
                {
                    object data = obj.Properties["processid"].Value;
                    if (data != null)
                    {
                        // retrieve the process
                        var childId = Convert.ToInt32(data);
                        var childProcess = Process.GetProcessById(childId);

                        // ensure the current process is still live
                        if (childProcess != null)
                        {
                            ProcessEntry p = new ProcessEntry()
                            {
                                Name = childProcess.ProcessName,
                                Pid = childProcess.Id,
                                Cpu = 0,
                                Memory = 0,
                                Gpu = 0,
                                Disk = 0,
                                Network = 0,
                                IsApplication = childProcess.MainWindowHandle != IntPtr.Zero ? true : false
                            };
                            childProcesses.Add(p);
                            results.Add(childProcess);
                        }
                    }
                }
            }
            return childProcesses;
        }

        /// <summary>
        /// Update CPU usage for a process list
        /// </summary>
        public double UpdateCpu(ObservableCollection<ProcessEntry> procList)
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
                procList[i].Cpu = Math.Round(cpuUsage * 100, 2);
                totalCpu += cpuUsage;
            }

            return Math.Round(totalCpu * 100, 2);
        }

        /// <summary>
        /// Update memory usage for a process list
        /// </summary>
        public double UpdateMem(ObservableCollection<ProcessEntry> procList)
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
                    procList[i].Memory = Math.Round((memoryUsages[i] / (double)totalMem) * 100, 2);
                    totalUsed += (ulong)memoryUsages[i];
                }
                else
                {
                    procList[i].Memory = -1;
                }
            }

            return Math.Round(((double)totalUsed / (double)totalMem) * 100, 2);
        }


        /// <summary>
        /// Update gpu usage for a process list
        /// </summary>
        public double UpdateGpu(ObservableCollection<ProcessEntry> procList)
        {
            Thread.Sleep(250);
            double totalLoad = 0;

            return totalLoad;
        }
    }
}

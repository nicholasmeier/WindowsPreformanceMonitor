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
using System.Runtime.InteropServices;
using System.Management;
using PerformanceMonitor.Cpp.CLI;

namespace WindowsPerformanceMonitor.Backend
{
    class Processes
    {
        public ObservableCollection<ProcessEntry> FindDelta(ObservableCollection<ProcessEntry> prev)
        {
            if (prev == null)
            {
                return GetProcesses();
            }

            // Load a hashset where Key = PID, Value is ProcessEntry
            Process[] processes = Process.GetProcesses();
            Dictionary<int, ProcessEntry> dict = new Dictionary<int, ProcessEntry>();
            for (int i = 0; i < prev.Count; i++)
            {
                dict.Add(prev[i].Pid, prev[i]);
            }

            /*
             * While iterating the new process list from GetProcess(), check if we already 
             * have a ProcessEntry for it. If so, just reuse it. Otherwise create a new one.
             */
            ObservableCollection<ProcessEntry> ret = new ObservableCollection<ProcessEntry>();
            foreach (Process proc in processes)
            {
                ProcessEntry temp;
                bool hasValue = dict.TryGetValue(proc.Id, out temp);
                if (hasValue)
                {
                    ret.Add(temp);
                    dict.Remove(proc.Id);
                }
                else
                {
                    ret.Add(new ProcessEntry()
                    {
                        Name = proc.ProcessName,
                        Pid = proc.Id,
                        Cpu = 0,
                        Gpu = 0,
                        Disk = 0,
                        Network = 0,
                        Ppid = GetParentProcess(proc.Id),
                        ChildProcesses = new List<ProcessEntry>(),
                        IsApplication = proc.MainWindowHandle != IntPtr.Zero ? true : false,
                        PrevCpu = new Tuple<DateTime, TimeSpan>(new DateTime(1), new TimeSpan(0))
                    });
                }
            }

            return ret;
        }

        /// <summary>
        /// Get list of processes of type ProcessEntry
        /// </summary>
        public ObservableCollection<ProcessEntry> GetProcesses()
        {
            
            ObservableCollection<ProcessEntry> processEntries = new ObservableCollection<ProcessEntry>();

            Process[] processes = Process.GetProcesses();

            if (processes.Length % 8 == 0)
            {
                ObservableCollection<ProcessEntry> l1 = new ObservableCollection<ProcessEntry>();
                ObservableCollection<ProcessEntry> l2 = new ObservableCollection<ProcessEntry>();
                ObservableCollection<ProcessEntry> l3 = new ObservableCollection<ProcessEntry>();
                ObservableCollection<ProcessEntry> l4 = new ObservableCollection<ProcessEntry>();
                ObservableCollection<ProcessEntry> l5 = new ObservableCollection<ProcessEntry>();
                ObservableCollection<ProcessEntry> l6 = new ObservableCollection<ProcessEntry>();
                ObservableCollection<ProcessEntry> l7 = new ObservableCollection<ProcessEntry>();
                ObservableCollection<ProcessEntry> l8 = new ObservableCollection<ProcessEntry>();
                Parallel.Invoke(
                    () => l1 = parallelList(processes, l1, 0, processes.Length / 8),
                    () => l2 = parallelList(processes, l2, (processes.Length / 8), (processes.Length * 2 / 8)),
                    () => l3 = parallelList(processes, l3, (processes.Length * 2 / 8), (processes.Length * 3 / 8)),
                    () => l4 = parallelList(processes, l4, (processes.Length * 3 / 8), (processes.Length * 4 / 8)),
                    () => l5 = parallelList(processes, l5, (processes.Length * 4 / 8), (processes.Length * 5 / 8)),
                    () => l6 = parallelList(processes, l6, (processes.Length * 5 / 8), (processes.Length * 6 / 8)),
                    () => l7 = parallelList(processes, l7, (processes.Length * 6 / 8), (processes.Length * 7 / 8)),
                    () => l8 = parallelList(processes, l8, (processes.Length * 7 / 8), (processes.Length))
                    );

                processEntries = new ObservableCollection<ProcessEntry>(l1.Concat(l2).Concat(l3).Concat(l4).Concat(l5).Concat(l6).Concat(l7).Concat(l8));
            }
            else if (processes.Length % 8 != 0)
            {
                ObservableCollection<ProcessEntry> l0 = new ObservableCollection<ProcessEntry>();
                int remainder = (processes.Length % 8);
                l0 = parallelList(processes, l0, 0, remainder);
                int newLen = processes.Length - remainder;
                ObservableCollection<ProcessEntry> l1 = new ObservableCollection<ProcessEntry>();
                ObservableCollection<ProcessEntry> l2 = new ObservableCollection<ProcessEntry>();
                ObservableCollection<ProcessEntry> l3 = new ObservableCollection<ProcessEntry>();
                ObservableCollection<ProcessEntry> l4 = new ObservableCollection<ProcessEntry>();
                ObservableCollection<ProcessEntry> l5 = new ObservableCollection<ProcessEntry>();
                ObservableCollection<ProcessEntry> l6 = new ObservableCollection<ProcessEntry>();
                ObservableCollection<ProcessEntry> l7 = new ObservableCollection<ProcessEntry>();
                ObservableCollection<ProcessEntry> l8 = new ObservableCollection<ProcessEntry>();
                int s1, e1, s2, s3, s4, s5, s6, s7, s8;
                s1 = remainder;
                e1 = (newLen / 8) + remainder; // because we need it be a multiple of 8 to split
                // everything must be offset by the remainder to assure alignment
                s2 = e1;
                s3 = (newLen * 2 / 8) + remainder;
                s4 = (newLen * 3 / 8) + remainder;
                s5 = (newLen * 4 / 8) + remainder;
                s6 = (newLen * 5 / 8) + remainder;
                s7 = (newLen * 6 / 8) + remainder;
                s8 = (newLen * 7 / 8) + remainder;
                Parallel.Invoke(
                    () => l1 = parallelList(processes, l1, s1, e1),
                    () => l2 = parallelList(processes, l2, s2, s3),
                    () => l3 = parallelList(processes, l3, s3, s4),
                    () => l4 = parallelList(processes, l4, s4, s5),
                    () => l5 = parallelList(processes, l5, s5, s6),
                    () => l6 = parallelList(processes, l6, s6, s7),
                    () => l7 = parallelList(processes, l7, s7, s8),
                    () => l8 = parallelList(processes, l8, s8, (processes.Length))
                    );
                processEntries = new ObservableCollection<ProcessEntry>(l0.Concat(l1).Concat(l2).Concat(l3).Concat(l4).Concat(l5).Concat(l6).Concat(l7).Concat(l8));
            }

            return processEntries;
        }

        internal void test()
        {
            throw new NotImplementedException();
        }

        private ObservableCollection<ProcessEntry> parallelList(Process[] processes, ObservableCollection<ProcessEntry> List, int start, int stop)
        {
            for (int i=start; i < stop; i++)
            {
                int ppid = GetParentProcess(processes[i].Id);
                ProcessEntry p = new ProcessEntry()
                {
                    Name = processes[i].ProcessName,
                    Pid = processes[i].Id,
                    Cpu = 0,
                    Gpu = 0,
                    Disk = 0,
                    Network = 0,
                    Ppid = ppid,
                    ChildProcesses = new List<ProcessEntry>(),
                    IsApplication = processes[i].MainWindowHandle != IntPtr.Zero ? true : false,
                    PrevCpu = new Tuple<DateTime, TimeSpan>(new DateTime(1), new TimeSpan(0))
                };

                if (p.Name.Length == 0)
                {
                    continue;
                }

                if(p.Name != "Idle")
                {
                    try
                    {
                        p.ApplicationName = processes[i].MainModule.ModuleName;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e + "parallelList");
                        p.ApplicationName = "Access Denied";
                        continue;
                    }

                    List.Add(p);
                }
            }
            return List;
        }

        public List<ProcessEntry> BuildProcessTree(List<ProcessEntry> list)
        {
            Dictionary<int, ProcessEntry> dict = new Dictionary<int, ProcessEntry>();
            for (int i = 0; i < list.Count; i++)
            {
                dict[list[i].Pid] = list[i];
            }

            Dictionary<int, ProcessEntry> tree = new Dictionary<int, ProcessEntry>();
            foreach (KeyValuePair<int, ProcessEntry> entry in dict)
            {
                if (entry.Value.Ppid == -1)
                {
                    tree[entry.Value.Pid] = entry.Value;
                }
            }

            foreach (KeyValuePair<int, ProcessEntry> entry in dict)
            {
                if (entry.Value.Ppid != -1)
                {
                    if (tree.ContainsKey(entry.Value.Ppid))
                    {
                        tree[entry.Value.Ppid].ChildProcesses.Add(entry.Value);
                    }
                    else if (dict.ContainsKey(entry.Value.Ppid))
                    {
                        if (tree.ContainsKey(dict[entry.Value.Ppid].Ppid))
                        {
                            tree[dict[entry.Value.Ppid].Ppid].ChildProcesses.Add(dict[entry.Value.Ppid]);
                            tree[dict[entry.Value.Ppid].Ppid].ChildProcesses[0].ChildProcesses.Add(entry.Value);
                        }
                        else
                        {
                            tree[entry.Value.Ppid] = dict[entry.Value.Ppid];
                            tree[entry.Value.Ppid].ChildProcesses.Add(entry.Value);
                        }
                    }
                    else
                    {
                        tree[entry.Value.Pid] = entry.Value;
                    }
                }
            }

            list.Clear();
            foreach (KeyValuePair<int, ProcessEntry> entry in tree)
            {
                list.Add(entry.Value);
            }

            return list;
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
        /// Update CPU usage for a process list
        /// </summary>
        public double UpdateCpu(ObservableCollection<ProcessEntry> procList)
        {
            List<DateTime> lastTimes = new List<DateTime>(new DateTime[procList.Count]);
            List<TimeSpan> lastTotalProcessorTime = new List<TimeSpan>(new TimeSpan[procList.Count]);

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
                    catch (Exception e)       // WIN32 access denied
                    {
                        Console.WriteLine(e + "cpu");
                        lastTotalProcessorTime.Insert(i, new TimeSpan(0));
                    }
                }
            }

            Thread.Sleep(10);
            double totalCpu = 0;

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
                catch (Exception e)            // WIN32 access denied.
                {
                    Console.WriteLine(e + "cpu2");
                    currTotalProcessorTime = new TimeSpan(0);
                }

                double cpuUsage = (currTotalProcessorTime.TotalMilliseconds - lastTotalProcessorTime[i].TotalMilliseconds) / currTime.Subtract(lastTimes[i]).TotalMilliseconds / Convert.ToDouble(Environment.ProcessorCount);
                procList[i].Cpu = Math.Round(cpuUsage * 100, 2);
                totalCpu += cpuUsage;
            }
            UpdateGpu(procList);
            return Math.Round(totalCpu * 100, 2);
        }

        /// <summary>
        /// Update memory usage for a process list
        /// </summary>
        public double UpdateMem(ObservableCollection<ProcessEntry> procList)
        {
            //var wmiObject = new ManagementObjectSearcher("select * from Win32_OperatingSystem");

            //var memoryValues = wmiObject.Get().Cast<ManagementObject>().Select(mo => new {
            //    FreePhysicalMemory = Double.Parse(mo["FreePhysicalMemory"].ToString()),
            //    TotalVisibleMemorySize = Double.Parse(mo["TotalVisibleMemorySize"].ToString())
            //}).FirstOrDefault();

            //if (memoryValues != null)
            //{
            //    return ((memoryValues.TotalVisibleMemorySize - memoryValues.FreePhysicalMemory) / memoryValues.TotalVisibleMemorySize) * 100;
            //}
            //return 0.0;
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
                    catch (Exception e)       // The platform is Windows 98 or Windows Millennium Edition which is not supported
                    {
                        Console.WriteLine(e + "mem");
                        memoryUsages.Insert(i, 0);
                    }
                }
            }

            Thread.Sleep(10);
            ulong totalMem = new Microsoft.VisualBasic.Devices.ComputerInfo().TotalPhysicalMemory;
            ulong totalUsed = 0;
            for (int i = 0; i < procList.Count; i++)
            {
                Process p;
                try
                {
                    p = Process.GetProcessById(procList[i].Pid);
                }
                catch (ArgumentException e)    // Process no longer running
                {
                    Console.WriteLine(e + "mem2");
                    procList[i].Memory = -1;
                    continue;
                }
                if (memoryUsages[i] == 0)
                {
                    procList[i].Memory = 0;
                }
                else if (memoryUsages[i] > 0)
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
            //Using a best guess estimate for now 
            //Estimate GPU usage using CPU usage
            double totalLoad = 0;
            for (int i = 0; i < procList.Count; i++)
            {
                procList[i].Gpu = procList[i].Cpu * 0.33;
                totalLoad += procList[i].Gpu;
            }
            return totalLoad;
        }


        //inner enum used only internally
        [Flags]
        private enum SnapshotFlags : uint
        {
            HeapList = 0x00000001,
            Process = 0x00000002,
            Thread = 0x00000004,
            Module = 0x00000008,
            Module32 = 0x00000010,
            Inherit = 0x80000000,
            All = 0x0000001F,
            NoHeaps = 0x40000000
        }
        //inner struct used only internally
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private struct PROCESSENTRY32
        {
            const int MAX_PATH = 260;
            internal UInt32 dwSize;
            internal UInt32 cntUsage;
            internal UInt32 th32ProcessID;
            internal IntPtr th32DefaultHeapID;
            internal UInt32 th32ModuleID;
            internal UInt32 cntThreads;
            internal UInt32 th32ParentProcessID;
            internal Int32 pcPriClassBase;
            internal UInt32 dwFlags;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_PATH)]
            internal string szExeFile;
        }

        [DllImport("kernel32", SetLastError = true, CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        static extern IntPtr CreateToolhelp32Snapshot([In]UInt32 dwFlags, [In]UInt32 th32ProcessID);

        [DllImport("kernel32", SetLastError = true, CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        static extern bool Process32First([In]IntPtr hSnapshot, ref PROCESSENTRY32 lppe);

        [DllImport("kernel32", SetLastError = true, CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        static extern bool Process32Next([In]IntPtr hSnapshot, ref PROCESSENTRY32 lppe);

        [DllImport("kernel32", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool CloseHandle([In] IntPtr hObject);

        // get the parent process given a pid
        public static int GetParentProcess(int pid)
        {
            Process parentProc = null;
            IntPtr handleToSnapshot = IntPtr.Zero;
            try
            {
                PROCESSENTRY32 procEntry = new PROCESSENTRY32();
                procEntry.dwSize = (UInt32)Marshal.SizeOf(typeof(PROCESSENTRY32));
                handleToSnapshot = CreateToolhelp32Snapshot((uint)SnapshotFlags.Process, 0);
                if (Process32First(handleToSnapshot, ref procEntry))
                {
                    do
                    {
                        if (pid == procEntry.th32ProcessID)
                        {
                            parentProc = Process.GetProcessById((int)procEntry.th32ParentProcessID);
                            break;
                        }
                    } while (Process32Next(handleToSnapshot, ref procEntry));
                }
                else
                {
                    throw new ApplicationException(string.Format("Failed with win32 error code {0}", Marshal.GetLastWin32Error()));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            finally
            {
                // Must clean up the snapshot object!
                CloseHandle(handleToSnapshot);
            }

            if (parentProc != null)
                return parentProc.Id;
            return -1;

            //try
            //{
            //    return parentProc.Id;

            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine(e);
            //    return -1;
            //}
        }

        public double updateDisk(ObservableCollection<ProcessEntry> procList)
        {
            List<float> diskUsages = new List<float>(new float[procList.Count]);

            for (int i = 0; i < procList.Count; i++)
            {
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
                    try
                    {
                        PerformanceCounter pc = new PerformanceCounter("Process", "IO Data Bytes/sec", p.ProcessName);
                        procList[i].Disk = pc.NextValue();
                    }
                    catch (Exception)       
                    {
                        diskUsages.Insert(i, 0);
                    }
                }
            }
            PerformanceCounter pt = new PerformanceCounter("Process", "IO Data Bytes/sec", "_Total");
            
            return pt.NextValue();
        }
    }
}

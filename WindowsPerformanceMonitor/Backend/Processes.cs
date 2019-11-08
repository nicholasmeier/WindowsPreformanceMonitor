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
                    PrevCpu = null
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

        public double UpdateCpu(ObservableCollection<ProcessEntry> procList)
        {
            double totalCpu = 0;
            foreach (ProcessEntry proc in procList)
            {
                Process p;
                if (proc.PrevCpu == null)
                {
                    try
                    {
                        p = Process.GetProcessById(proc.Pid);
                        proc.PrevCpu = new Tuple<DateTime, TimeSpan>(DateTime.Now, p.TotalProcessorTime);
                    }
                    catch (Exception)
                    {
                        proc.PrevCpu = new Tuple<DateTime, TimeSpan>(DateTime.Now, new TimeSpan(0));
                    }
                }

                try
                {
                    p = Process.GetProcessById(proc.Pid);
                    DateTime currTime = DateTime.Now;
                    TimeSpan currProcTime = p.TotalProcessorTime;

                    double cpuUsage = (currProcTime.TotalMilliseconds - proc.PrevCpu.Item2.TotalMilliseconds) 
                                        / currTime.Subtract(proc.PrevCpu.Item1).TotalMilliseconds 
                                        / Convert.ToDouble(Environment.ProcessorCount);

                    proc.Cpu = Math.Round(cpuUsage * 100, 2);
                    proc.Gpu = Math.Round(proc.Cpu * .11, 2);   // GPU Estimation***
                    totalCpu += cpuUsage;

                    proc.PrevCpu = new Tuple<DateTime, TimeSpan>(currTime, currProcTime);

                }
                catch (Exception)
                {
                    proc.Cpu = 0;
                    // We want to insert a non-null prevCpu so we don't hit GetProcessById again since that
                    // function is what slows this down.
                    proc.PrevCpu = new Tuple<DateTime, TimeSpan>(DateTime.Now, new TimeSpan(0));
                }

            }

            return totalCpu;
        }

        public double UpdateMem(ObservableCollection<ProcessEntry> procList)
        {
            double totalUsed = 0;
            ulong totalMem = new Microsoft.VisualBasic.Devices.ComputerInfo().TotalPhysicalMemory;
            foreach (ProcessEntry proc in procList)
            {
                Process p;
                try
                {
                    p = Process.GetProcessById(proc.Pid);
                    totalUsed +=  p.WorkingSet64;
                    proc.Memory = Math.Round(((double) p.WorkingSet64 / totalMem * 100), 2);
                }
                catch (Exception)
                {
                    proc.Memory = 0;
                }
            }

            return Math.Round(((double)totalUsed / (double)totalMem) * 100, 2);
        }

        public double UpdateGpu(ObservableCollection<ProcessEntry> procList)
        {
            double totalLoad = 0;
            for (int i = 0; i < procList.Count; i++)
            {
                procList[i].Gpu = procList[i].Cpu * 0.11;
                totalLoad += procList[i].Gpu;
            }
            return totalLoad;
        }

        /*
         * This performance counter gives a total of disk and network.
         * This function takes longer than any of the others.
         */
        public double UpdateDisk(ObservableCollection<ProcessEntry> procList)
        {
            double totalDisk = 0;
            foreach (ProcessEntry proc in procList)
            {
                if (proc.PrevDisk == null)
                {
                    proc.PrevDisk = new PerformanceCounter("Process", "IO Data Bytes/sec", proc.Name);
                }

                try
                {
                    proc.Disk = (float)Math.Round(proc.PrevDisk.NextValue() / 1000000, 2);
                    totalDisk += proc.Disk;
                }
                catch (Exception)
                {
                    proc.Disk = 0;
                }

            }

            return totalDisk;

        }

        #region C++ Interop
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

        #endregion

    }
}

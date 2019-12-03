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
using System.Windows.Forms;
using System.Drawing;
using System.Windows.Interop;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace WindowsPerformanceMonitor.Backend
{
    class Processes
    {
        WindowsNotification WindowsNotification = new WindowsNotification();

        struct IO_COUNTERS
        {
            public ulong ReadOperationCount;
            public ulong WriteOperationCount;
            public ulong OtherOperationCount;
            public ulong ReadTransferCount;
            public ulong WriteTransferCount;
            public ulong OtherTransferCount;
        }
        [DllImport(@"kernel32.dll", SetLastError = true)]
        static extern bool GetProcessIoCounters(IntPtr hProcess, out IO_COUNTERS counters);


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
                    var p = new ProcessEntry()
                    {
                        Name = proc.ProcessName,
                        Proc = proc,
                        Pid = proc.Id,
                        Cpu = 0,
                        Gpu = 0,
                        Disk = 0,
                        Network = 0,
                        Ppid = GetParentProcess(proc.Id),
                        ChildProcesses = new List<ProcessEntry>(),
                        IsApplication = proc.MainWindowHandle != IntPtr.Zero ? true : false,
                        PrevCpu = new Tuple<DateTime, TimeSpan>(new DateTime(1), new TimeSpan(0)),
                    };

                    try
                    {
                        p.ApplicationName = proc.MainModule.ModuleName;
                        p.ApplicationPath = proc.MainModule.FileName;
                    }
                    catch (Exception)
                    {

                    }

                    ret.Add(p);
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
            for (int i = start; i < stop; i++)
            {
                ProcessEntry p = null;
                try
                {
                    int ppid = GetParentProcess(processes[i].Id);
                    p = new ProcessEntry()
                    {
                        Name = processes[i].ProcessName,
                        Pid = processes[i].Id,
                        Proc = Process.GetProcessById(processes[i].Id),
                        Cpu = 0,
                        Gpu = 0,
                        Disk = 0,
                        Network = 0,
                        Ppid = ppid,
                        ChildProcesses = new List<ProcessEntry>(),
                        IsApplication = processes[i].MainWindowHandle != IntPtr.Zero ? true : false,
                        PrevCpu = null,
                    };
                }
                catch (Exception)
                {
                    //
                    continue;
                }

                if (p.Name.Length == 0)
                {
                    continue;
                }

                if (p.Name != "Idle")
                {
                    try
                    {
                        p.ApplicationName = processes[i].MainModule.ModuleName;
                        p.ApplicationPath = processes[i].MainModule.FileName;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e + "parallelList");
                        p.ApplicationName = "Access Denied";
                        p.ApplicationPath = "Access Denied";
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
                if (proc.PrevCpu == null)
                {
                    try
                    {
                        proc.PrevCpu = new Tuple<DateTime, TimeSpan>(DateTime.Now, proc.Proc.TotalProcessorTime);
                    }
                    catch (Exception)
                    {
                        proc.PrevCpu = new Tuple<DateTime, TimeSpan>(DateTime.Now, new TimeSpan(0));
                    }
                }

                try
                {
                    DateTime currTime = DateTime.Now;
                    TimeSpan currProcTime = proc.Proc.TotalProcessorTime;

                    double cpuUsage = (currProcTime.TotalMilliseconds - proc.PrevCpu.Item2.TotalMilliseconds)
                                        / currTime.Subtract(proc.PrevCpu.Item1).TotalMilliseconds
                                        / Convert.ToDouble(Environment.ProcessorCount);

                    proc.Cpu = Math.Round(cpuUsage * 100, 2);
                    if (proc.Cpu > 0)
                    {
                        if (proc.Cpu > 0 && proc.cpuThreshold > 0)
                        {
                            if (proc.Cpu > proc.cpuThreshold)
                            {
                                if (proc.cpuThresholdPassedTime == (new DateTime()))
                                {
                                    // first time threshold has gone over
                                    string message = proc.Name + " is using " + proc.Cpu + "% of the cpu";
                                    WindowsNotification.ShowNotification("Process CPU Notification", message);
                                    proc.cpuThresholdPassedTime = DateTime.Now;
                                }
                                else
                                {
                                    TimeSpan timeSpan = DateTime.Now - proc.cpuThresholdPassedTime;
                                    if (timeSpan.TotalMinutes > 2)
                                    {
                                        string message = proc.Name + " is using " + proc.Cpu + "% of the cpu";
                                        WindowsNotification.ShowNotification("Process CPU Notification", message);
                                        proc.cpuThresholdPassedTime = DateTime.Now;
                                    }
                                }
                            }
                        }
                    }

                    proc.Gpu = Math.Round(proc.Cpu * .11, 2);   // GPU Estimation***
                    if (proc.Gpu > 0)
                    {
                        if (proc.Gpu > 0 && proc.gpuThreshold > 0)
                        {
                            if (proc.Gpu > proc.gpuThreshold)
                            {
                                if (proc.gpuThresholdPassedTime == (new DateTime()))
                                {
                                    // first time threshold has gone over
                                    string message = proc.Name + " is using " + proc.Gpu + "% of the gpu";
                                    WindowsNotification.ShowNotification("Process GPU Notification", message);
                                    proc.gpuThresholdPassedTime = DateTime.Now;
                                }
                                else
                                {
                                    TimeSpan timeSpan = DateTime.Now - proc.gpuThresholdPassedTime;
                                    if (timeSpan.TotalMinutes > 2)
                                    {
                                        string message = proc.Name + " is using " + proc.Cpu + "% of the gpu";
                                        WindowsNotification.ShowNotification("Process GPU Notification", message);
                                        proc.gpuThresholdPassedTime = DateTime.Now;
                                    }
                                }
                            }
                        }
                    }
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

            return Math.Round(totalCpu * 100, 2);
        }

        public double UpdateMem(ObservableCollection<ProcessEntry> procList)
        {
            double totalUsed = 0;
            ulong totalMem = new Microsoft.VisualBasic.Devices.ComputerInfo().TotalPhysicalMemory;
            foreach (ProcessEntry proc in procList)
            {
                try
                {
                    totalUsed += proc.Proc.WorkingSet64;
                    proc.Memory = Math.Round(((double)proc.Proc.WorkingSet64 / totalMem * 100), 2);

                    if (proc.Memory > 0)
                    {
                        if (proc.Memory > 0 && proc.memoryThreshold > 0)
                        {
                            if (proc.Memory > proc.memoryThreshold)
                            {
                                if (proc.memoryThresholdPassedTime == (new DateTime()))
                                {
                                    // first time threshold has gone over
                                    string message = proc.Name + " is using " + proc.Cpu + "MB of the memory";
                                    WindowsNotification.ShowNotification("Process Memory Notification", message);
                                    proc.memoryThresholdPassedTime = DateTime.Now;
                                }
                                else
                                {
                                    TimeSpan timeSpan = DateTime.Now - proc.memoryThresholdPassedTime;
                                    if (timeSpan.TotalMinutes > 2)
                                    {
                                        string message = proc.Name + " is using " + proc.Cpu + "MB of the memory";
                                        WindowsNotification.ShowNotification("Process Memory Notification", message);
                                        proc.memoryThresholdPassedTime = DateTime.Now;
                                    }
                                }
                            }
                        }
                    }
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
        public double UpdateDisk(ComputerObj obj)
        {
            //Remove * lines if we are changing the network system.
            ObservableCollection<ProcessEntry> procList = obj.ProcessList;
            double totalDisk = 0;
            double totalNetwork = 0;//*
            foreach (ProcessEntry proc in procList)
            {
                if (proc.IsApplication)
                {//no other way to find disk usage without running into a ton of errors.
                    try
                    {
                        if (GetProcessIoCounters(proc.Proc.Handle, out IO_COUNTERS counters))
                        {
                            ulong RCount = counters.ReadTransferCount;
                            ulong WCount = counters.WriteTransferCount;
                            bool inf = false;
                            if (proc.PrevTime != null)
                            {
                                if ((System.DateTime.Now - proc.PrevTime).TotalMilliseconds > 20)
                                {
                                    float temp = (float)Math.Round(((RCount + WCount - proc.PrevDisk) / 1000000 /
                                        (System.DateTime.Now - proc.PrevTime).TotalSeconds), 2);
                                    if (float.IsNaN(temp))
                                        proc.Disk = 0;
                                    else if (float.IsInfinity(temp))
                                        inf = true;
                                    else proc.Disk = temp;
                                    //proc.Network = (float)Math.Round((counters.OtherTransferCount - proc.PrevNetwork) / 1000000 /
                                    //(System.DateTime.Now - proc.PrevTime).TotalSeconds, 2); //*
                                }
                            }
                            if (!inf)
                            {
                                proc.PrevTime = System.DateTime.Now;
                                proc.PrevDisk = RCount + WCount;
                                //proc.PrevNetwork = counters.OtherTransferCount;//*
                            }
                        }

                        totalDisk += proc.Disk;
                        //totalNetwork += proc.Network;//*
                    }
                    catch (Exception e)
                    {
                        string result = e.Message;
                        proc.Disk = 0;
                        //proc.Network = 0;
                    }
                }
                else
                {
                    proc.Disk = 0;
                }
            }
            obj.TotalNetwork = totalNetwork;
            totalDisk = Math.Round(totalDisk, 2);
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

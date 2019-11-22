using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenHardwareMonitor.Hardware;
using WindowsPerformanceMonitor;
using WindowsPerformanceMonitor.Backend;
using WindowsPerformanceMonitor.Models;

// Subscriber
public class ComputerStatsMonitor : IObservable<ComputerObj>
    {
    List<IObserver<ComputerObj>> observers;
    public int tabIndex = 0;
    NotificationThresholds notifications = new NotificationThresholds();
    private readonly NotifyIcon _notifyIcon;
    public ComputerStatsMonitor()
    {
        observers = new List<IObserver<ComputerObj>>();
        _notifyIcon = new NotifyIcon();
        // Extracts your app's icon and uses it as notify icon
        _notifyIcon.Icon = new System.Drawing.Icon("../../Graphics/WindowsPerformanceMonitor.ico");
        // Hides the icon when the notification is closed
        _notifyIcon.BalloonTipClosed += (s, e) => _notifyIcon.Visible = false;
    }
    private class Unsubscriber : IDisposable
    {
        private List<IObserver<ComputerObj>> _observers;
        private IObserver<ComputerObj> _observer;
        public Unsubscriber(List<IObserver<ComputerObj>> observers, IObserver<ComputerObj> observer)
        {
            this._observers = observers;
            this._observer = observer;
        }

        public void Dispose()
        {
            if (!(_observer == null)) _observers.Remove(_observer);
        }
    }
    public IDisposable Subscribe(IObserver<ComputerObj> observer)
    {
        if (!observers.Contains(observer))
            observers.Add(observer);

        return new Unsubscriber(observers, observer);
    }

    public class UpdateVisitor : IVisitor
    {
        public void VisitComputer(IComputer computer)
        {
            computer.Traverse(this);
        }
        public void VisitHardware(IHardware hardware)
        {
            hardware.Update();
            foreach (IHardware subHardware in hardware.SubHardware) subHardware.Accept(this);
        }
        public void VisitSensor(ISensor sensor) { }
        public void VisitParameter(IParameter parameter) { }
    }
    
    public double getTotalGpuLoad(Computer computer)
    {
        double load = 0;
        for (int i = 0; i < computer.Hardware.Length; i++)
        {
            if (computer.Hardware[i].HardwareType == HardwareType.GpuAti || computer.Hardware[i].HardwareType == HardwareType.GpuNvidia)
            {
                for (int j = 0; j < computer.Hardware[i].Sensors.Length; j++)
                {
                    if (computer.Hardware[i].Sensors[j].SensorType == SensorType.Load)
                        load = (double)computer.Hardware[i].Sensors[j].Value;
                }
            }
        }

        return load;
    }

    public void getComputerInformation() //this should only be called one time on initialization in its own background thread.
    {
        UpdateVisitor updateVisitor = new UpdateVisitor();
        Computer computer = new Computer();
        ComputerObj obj = new ComputerObj();
        ObservableCollection<ProcessEntry> plist = null;
        Processes processes = new Processes();
        obj.Computer = computer;
        obj.ProcessList = plist;
        computer.Open();
        computer.CPUEnabled = true;
        computer.GPUEnabled = true;
        computer.RAMEnabled = true;
        computer.HDDEnabled = true;
        computer.FanControllerEnabled = true;
        computer.MainboardEnabled = true;
        Task diskTask = null;

        while (true)
        {
            obj.ProcessList = processes.FindDelta(obj.ProcessList);

            /*
             * This function is slow, so this should let it run independently. It 
             * will not bottleneck the other updates if we have it run it its own
             * Task.
             */
            if (diskTask == null || diskTask.IsCompleted)
            {
                diskTask = new Task(() => {
                    obj.TotalDisk = processes.UpdateDisk(obj);
                });

                diskTask.Start();
            }

            if (tabIndex == 1)
            {
                Parallel.Invoke(
                    () => obj.TotalCpu = processes.UpdateCpu(obj.ProcessList),
                    () => obj.TotalMemory = processes.UpdateMem(obj.ProcessList),
                    () => obj.ProcessTree = new ObservableCollection<ProcessEntry>(processes.BuildProcessTree(new List<ProcessEntry>(processes.GetProcesses()))),
                    () => obj.Tab = 1
                );
            }
            else
            {
                Parallel.Invoke(
                    () => obj.TotalCpu = processes.UpdateCpu(obj.ProcessList),
                    () => obj.TotalMemory = processes.UpdateMem(obj.ProcessList),
                    () => obj.Tab = 0
                ); ;
            }

            checkNotificationThresholds(obj.TotalCpu, obj.TotalGpu, obj.TotalMemory);
            checkLogSchedule();

            computer.Accept(updateVisitor);
            Parallel.ForEach(observers, observer =>
                observer.OnNext(obj)
            );
        }
    }
    
    public void checkLogSchedule()
    {
        List<Tuple<string, string>> logList = (List<Tuple<string, string>>)App.Current.Properties["ScheduledLogList"];
        string currTime = DateTime.Now.ToString("h:mm tt");
        bool running = false;
        if (logList != null)
        {
            if (App.Current.Properties["ScheduledLogRunning"] != null)
            {
                running = (bool)App.Current.Properties["ScheduledLogRunning"];
            }
            foreach (var logtime in logList)
            {
                if (currTime == logtime.Item1 && !running)
                {
                    App.Current.Properties["ScheduledLogRunning"] = true;
                    // start the log 
                    Globals._log.StartLog();
                }
                else if (currTime == logtime.Item2)
                {
                    if (Globals._log != null)
                    {
                        Globals._log.WriteIt();
                    }
                    App.Current.Properties["ScheduledLogRunning"] = false;
                    Globals._log = new Log();
                    logList.Remove(logtime);
                    App.Current.Properties["ScheduledLogList"] = logList;
                    if (logList.Count == 0)
                    {
                        break;
                    }
                }
            }
        }
            


    }

    public void checkNotificationThresholds(double Totalcpu, double Totalgpu, double Totalmemory)
    {
        notifications.cpuThreshold = Convert.ToDouble(App.Current.Properties["cpuThreshold"]);
        notifications.gpuThreshold = Convert.ToDouble(App.Current.Properties["gpuThreshold"]);
        notifications.memoryThreshold = Convert.ToDouble(App.Current.Properties["memoryThreshold"]);

        if (Totalcpu > 0 && notifications.cpuThreshold > 0)
        {
            if (Totalcpu > notifications.cpuThreshold)
            {
                if (notifications.cpuThresholdPassed != true)
                {

                    //notifications.cpuThresholdPassed = true;
                    if (notifications.cpuThresholdPassedTime == (new DateTime()))
                    {
                        // first time the threshold has ever been passed
                        string message = "CPU is at " + Totalcpu + "%";
                        ShowNotification("CPU Threshold Notification", message);
                        notifications.cpuThresholdPassedTime = DateTime.Now;
                    }
                    else
                    {
                        // the threshold has been hit before we need to assert it has been atleast two minute later
                        TimeSpan timeSpan = DateTime.Now - notifications.cpuThresholdPassedTime;
                        if (timeSpan.TotalMinutes > 2)
                        {
                            // it has been two minutes lets make another notification
                            string message = "CPU is at " + Totalcpu + "%";
                            ShowNotification("CPU Threshold Notification", message);
                            notifications.cpuThresholdPassedTime = DateTime.Now;
                        }
                    }
                }
            }
        }

        if (Totalgpu > 0 && notifications.gpuThreshold > 0)
        {
            if (Totalgpu > notifications.gpuThreshold)
            {
                if (notifications.gpuThresholdPassedTime == (new DateTime()))
                {
                    // first time the threshold has ever been passed
                    string message = "GPU is at " + Totalgpu + "%";
                    ShowNotification("GPU Threshold Notification", message);
                    notifications.gpuThresholdPassedTime = DateTime.Now;
                }
                else
                {
                    // the threshold has been hit before we need to assert it has been atleast two minutes later
                    TimeSpan timeSpan = DateTime.Now - notifications.gpuThresholdPassedTime;
                    if (timeSpan.TotalMinutes > 2)
                    {
                        string message = "GPU is at " + Totalgpu + "%";
                        ShowNotification("GPU Threshold Notification", message);
                        notifications.gpuThresholdPassedTime = DateTime.Now;
                    }
                }
            }
        }

        if (Totalmemory > 0 && notifications.memoryThreshold > 0)
        {
            if (Totalmemory > notifications.memoryThreshold)
            {
                if (notifications.memoryThresholdPassedTime == (new DateTime()))
                {
                    // first time the threshold has ever passed
                    string message = "Memory is at " + Totalmemory + "%";
                    ShowNotification("Memory Threshold Notification", message);
                    notifications.memoryThresholdPassedTime = DateTime.Now;
                }
                else
                {
                    TimeSpan timeSpan = DateTime.Now - notifications.memoryThresholdPassedTime;
                    if (timeSpan.TotalMinutes > 2)
                    {
                        string message = "Memory is at " + Totalmemory + "%";
                        ShowNotification("Memory Threshold Notification", message);
                        notifications.memoryThresholdPassedTime = DateTime.Now;
                    }
                }
            }
        }
    }


    public void ShowNotification(string title, string message)
    {
        _notifyIcon.Visible = true;
        // Shows a notification with specified message and title
        _notifyIcon.ShowBalloonTip(50000, title, message, ToolTipIcon.Warning);
    }

}


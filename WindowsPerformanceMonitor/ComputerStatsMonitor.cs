using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using OpenHardwareMonitor.Hardware;
using WindowsPerformanceMonitor.Backend;
using WindowsPerformanceMonitor.Models;

// Subscriber
public class ComputerStatsMonitor : IObservable<ComputerObj>
    {
    List<IObserver<ComputerObj>> observers;
    public ComputerStatsMonitor()
    {
        observers = new List<IObserver<ComputerObj>>();
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
        while (true)
        {
            obj.ProcessList = new ObservableCollection<ProcessEntry>(processes.GetProcesses() as List<ProcessEntry>);

            // Ideally, we let these fire off on their own and it just gets updated in UI when the processEntry updates, but for now I'm just
            // going to await them all.

            List<Task> tasks = new List<Task>();
            tasks.Add(new Task(() =>
            {
                updateCpu(obj.ProcessList);
                // updateMem, etc.
            }));

            Parallel.ForEach(tasks, task => task.Start());
            Task.WaitAll(tasks.ToArray());

            computer.Accept(updateVisitor);
            foreach (var observer in observers) observer.OnNext(obj);
            Thread.Sleep(1000);
        }
    }

    public void updateCpu(ObservableCollection<ProcessEntry> procList)
    {
        List<DateTime> lastTimes = new List<DateTime>( new DateTime[procList.Count] );
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
        }
    }

}


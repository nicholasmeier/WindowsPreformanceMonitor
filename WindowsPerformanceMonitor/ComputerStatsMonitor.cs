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
        while (true)
        {
            List<ProcessEntry> list = processes.GetProcesses();
            obj.ProcessList = new ObservableCollection<ProcessEntry>(new List<ProcessEntry>(list));
            List<Task> tasks = new List<Task>();
            tasks.Add(new Task(() =>
            {
                obj.TotalCpu = processes.UpdateCpu(obj.ProcessList);
                obj.TotalMemory = processes.UpdateMem(obj.ProcessList);
                obj.TotalGpu = getTotalGpuLoad(computer);
                obj.TotalDisk = processes.updateDisk(obj.ProcessList);
                obj.ProcessTree = new ObservableCollection<ProcessEntry>(processes.BuildProcessTree(new List<ProcessEntry>(list)));
            }));

            Parallel.ForEach(tasks, task => task.Start());
            Task.WaitAll(tasks.ToArray());
            computer.Accept(updateVisitor);
            foreach (var observer in observers) observer.OnNext(obj);
            Thread.Sleep(500);
        }
    }



    // Maybe put graph data in its own subscriber fnc so we can loop quicker?
}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenHardwareMonitor.Hardware;

    public class ComputerStatsMonitor : IObservable<Computer>
    {
    List<IObserver<Computer>> observers;
    public ComputerStatsMonitor()
    {
        observers = new List<IObserver<Computer>>();
    }
    private class Unsubscriber : IDisposable
    {
        private List<IObserver<Computer>> _observers;
        private IObserver<Computer> _observer;
        public Unsubscriber(List<IObserver<Computer>> observers, IObserver<Computer> observer)
        {
            this._observers = observers;
            this._observer = observer;
        }

        public void Dispose()
        {
            if (!(_observer == null)) _observers.Remove(_observer);
        }
    }
    public IDisposable Subscribe(IObserver<Computer> observer)
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
        computer.Open();
        computer.CPUEnabled = true;
        computer.GPUEnabled = true;
        computer.RAMEnabled = true;
        computer.HDDEnabled = true;
        computer.FanControllerEnabled = true;
        while (true)
        {
           //sleep thread here to reduce information refreshing
            computer.Accept(updateVisitor);
            foreach (var observer in observers) observer.OnNext(computer);
        }
    }
}


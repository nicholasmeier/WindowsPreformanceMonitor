using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenHardwareMonitor.Hardware;
using WindowsPerformanceMonitor.Models;

// Observer

namespace WindowsPerformanceMonitor
{
    class HardwareObserver : IObserver<ComputerObj>
    {
        private IDisposable unsubscriber;
        Action<ComputerObj> _onNext;
        public HardwareObserver(Action<ComputerObj> onNext)
        {
            _onNext = onNext;
        }
        public virtual void Subscribe(IObservable<ComputerObj> provider)
        {
            unsubscriber = provider.Subscribe(this);
        }
        public virtual void Unsubscribe()
        {
            unsubscriber.Dispose();
        }
        public virtual void OnNext(ComputerObj value)
        {
            _onNext(value);
            //TODO: this is the method called every update.
        }

        public virtual void OnCompleted()
        {
            //not used, but part of interface
        }
        public virtual void OnError(Exception e)
        {
            //
        }
    }
}

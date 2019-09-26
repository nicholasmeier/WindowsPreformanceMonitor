using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenHardwareMonitor.Hardware;

namespace WindowsPerformanceMonitor
{
    class HardwareObserver : IObserver<Computer>
    {
        private IDisposable unsubscriber;
        Action<Computer> _onNext;
        public HardwareObserver(Action<Computer> onNext)
        {
            _onNext = onNext;
        }
        public virtual void Subscribe(IObservable<Computer> provider)
        {
            unsubscriber = provider.Subscribe(this);
        }
        public virtual void Unsubscribe()
        {
            unsubscriber.Dispose();
        }
        public virtual void OnNext(Computer value)
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

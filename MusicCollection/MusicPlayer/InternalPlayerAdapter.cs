using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows.Threading;

namespace MusicCollection.MusicPlayer
{
    internal abstract class InternalPlayerAdapter:IDisposable
    {
        private DispatcherTimer _Timer;

        protected InternalPlayerAdapter(double interval):this(interval,Dispatcher.CurrentDispatcher)
        {
        }

        protected InternalPlayerAdapter(double interval, Dispatcher iDispatcher)
        {
            _Timer = new DispatcherTimer(TimeSpan.FromMilliseconds(interval), DispatcherPriority.Normal, OnTime, iDispatcher);
        }

        private void OnTime(object sender, EventArgs ea)
        {
            OnTimer();
        }

        protected void TimerStart()
        {
            _Timer.Start();
        }

        protected void TimerStop()
        {
            _Timer.Stop();
        }

        abstract protected void DoStop();
        abstract protected void DoPlay();
        abstract protected void DoPause();
        abstract protected void DoClose();
        
        public virtual void Stop()
        {
            TimerStop();
            DoStop();
        }

        public void Play()
        {
            TimerStart();
            DoPlay();
        }

        public void Pause()
        {
            TimerStop();
             DoPause();
        }

        public void Close()
        {
            TimerStop();
            DoClose();
        }

        public virtual void Dispose()
        {
            _Timer.Tick -= OnTime;
            _Timer.Stop();
        }

        protected abstract void OnTimer();
    }
}

using System;
using System.Threading;

namespace EHaskins.Frc.Communication.DriverStation
{
    public class DSThreadingTimer : DSTimer
    {
        Timer _timer;

        public DSThreadingTimer() : base() { }
        public DSThreadingTimer(int interval) : base(interval) { }

        protected override void EnableTimer()
        {
            Interval = 20;
            TimerRunning = true;
            _timer = new Timer(TimerHandler, null, Interval, Interval);
        }
        protected override void DisableTimer()
        {
            _timer.Change(Timeout.Infinite, Timeout.Infinite);
            _timer.Dispose();
            _timer = null;
            TimerRunning = false;
        }
        private void TimerHandler(object state)
        {
            OnTimerElapsed();
        }

        protected override void InnerDispose()
        {
            if (_timer != null)
            {
                DisableTimer();
            }
            base.InnerDispose();
        }

        ~DSThreadingTimer()
        {
            Dispose();
        }
    }
}

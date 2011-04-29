using System;
using MicroLibrary;

namespace EHaskins.Frc.Communication.DriverStation
{
    public class DSMicroTimer : DSTimer
    {
        MicroTimer _timer;
        public DSMicroTimer() : base() { }
        public DSMicroTimer(int interval) : base(interval) { }

        protected override void EnableTimer()
        {
            Interval = 20;
            TimerRunning = true;
            if (_timer == null)
                _timer = new MicroTimer();

            _timer = new MicroTimer(Interval * 1000);
            _timer.Elapsed += TimerHandler;
            _timer.IgnoreEventIfLateBy = _timer.Interval / 2;
            _timer.Start();
        }
        protected override void DisableTimer()
        {
            TimerRunning = false;
            _timer.Stop();
        }
        private void TimerHandler(object sender, MicroTimerEventArgs timerEventArgs)
        {
            OnTimerElapsed();
        }

        protected override void InnerDispose()
        {
            if (_timer != null)
            {
                _timer.Stop();
                _timer = null;
            }
            base.InnerDispose();
        }

        ~DSMicroTimer()
        {
            Dispose();
        }
    }
}

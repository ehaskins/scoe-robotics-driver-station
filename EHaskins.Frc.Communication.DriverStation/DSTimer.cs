using System;
using System.Collections.Generic;

namespace EHaskins.Frc.Communication.DriverStation
{
    public abstract class DSTimer : IDisposable
    {
        int enabledDSs;
        public DSTimer() : this(20) { }
        public DSTimer(int interval)
        {
            _DriverStations = new List<DriverStation>();
            Interval = interval;
        }

        public event EventHandler TimerElapsing;
        protected void RaiseTimerElapsing(){
            if (TimerElapsing != null)
                TimerElapsing(this, null);
        }
                public event EventHandler TimerElapsed;
        protected void RaiseTimerElapsed(){
            if (TimerElapsed != null)
                TimerElapsed(this, null);
        }
        protected void OnTimerElapsed()
        {
            RaiseTimerElapsing();
            foreach (DriverStation driverStation in _DriverStations)
            {
                driverStation.SendData();
            }
            RaiseTimerElapsed();
        }

        public int EnabledDSs
        {
            get
            {
                return enabledDSs;
            }
            set
            {
                if (enabledDSs == value)
                    return;
                enabledDSs = value;

                if (EnabledDSs <= 0)
                {
                    EnabledDSs = 0;
                    if (TimerRunning)
                        DisableTimer();
                }
                else
                {
                    if (!TimerRunning)
                    {
                        EnableTimer();
                    }
                }
            }
        }
        public int Interval { get; protected set; }
        private List<DriverStation> _DriverStations;
        public DriverStation[] DriverStations
        {
            get { return _DriverStations.ToArray(); }
        }

        public void AddDriverStation(DriverStation ds)
        {
            if (!_DriverStations.Contains(ds))
            {
                _DriverStations.Add(ds);
                ds.Started += DSStarted;
                ds.Stopped += DSStopped;
                if (ds.IsEnabled)
                    EnabledDSs++;
            }
        }

        private void DSStarted(object sender, EventArgs e)
        {
            EnabledDSs++;

        }

        private void DSStopped(object sender, EventArgs e)
        {
            EnabledDSs--;
        }

        public bool TimerRunning { get; protected set; }
        protected abstract void EnableTimer();
        protected abstract void DisableTimer();

        private bool _disposed;
        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;
                InnerDispose();
            }
        }
        protected virtual void InnerDispose()
        {
            _DriverStations.Clear();
        }
    }
}

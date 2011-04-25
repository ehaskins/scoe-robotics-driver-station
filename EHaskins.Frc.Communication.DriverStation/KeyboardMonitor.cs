using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX.DirectInput;
using SlimDX;
namespace EHaskins.Frc.Communication.DriverStation
{
    public class KeyboardMonitor : IDisposable
    {
        DirectInput _input;
        Keyboard _keyboard;

        /// <summary>
        /// Initializes a new instance of the EStop class.
        /// </summary>
        public KeyboardMonitor()
        {
            DriverStations = new List<DriverStation>();
            //Start();
        }
        public event EventHandler EStopPressed;
        protected void RaiseEStopPressed()
        {
            if (EStopPressed != null)
                EStopPressed(this, null);
            foreach (DriverStation ds in DriverStations)
            {
                if (ds.IsEnabled)
                    ds.ControlData.Mode.IsEStop = true;
            }
        }
        public event EventHandler EnablePressed;
        protected void RaiseEnablePressed()
        {
            if (EnablePressed != null)
                EnablePressed(this, null);
            foreach (DriverStation ds in DriverStations)
            {
                if (ds.IsEnabled)
                    ds.ControlData.Mode.IsEnabled = true;
            }
        }
        public event EventHandler DisablePressed;
        protected void RaiseDisablePressed()
        {
            if (DisablePressed != null)
                DisablePressed(this, null);
            foreach (DriverStation ds in DriverStations)
            {
                if (ds.IsEnabled)
                    ds.ControlData.Mode.IsEnabled = false;
            }
        }

        public void Start()
        {
            _input = new DirectInput();
            _keyboard = new Keyboard(_input);
            _keyboard.Properties.BufferSize = 8;
            _keyboard.Acquire();

            foreach (DriverStation ds in DriverStations)
            {
                ds.SendingData += SendingDataHandler;   
            }
        }
        public void Stop()
        {
            _keyboard.Unacquire();
            _keyboard.Dispose();
            _keyboard = null;
            _input.Dispose();
            _input = null;
        }

        public List<DriverStation> DriverStations { get; set; }

        private void SendingDataHandler(object sender, EventArgs e)
        {
            Poll();
        }

        public void Poll()
        {
            if (_keyboard.Acquire().IsFailure)
                return;

            if (_keyboard.Poll().IsFailure)
                return;

            IEnumerable<KeyboardState> bufferedData = _keyboard.GetBufferedData();
            if (Result.Last.IsFailure)
                return;

            StringBuilder data = new StringBuilder();

            var keys = new List<Key>();
            foreach (KeyboardState packet in bufferedData)
            {
                foreach (Key key in packet.PressedKeys)
                    keys.Add(key);
            }

            if (keys.Contains(Key.F1))
                RaiseEnablePressed();
            if (keys.Contains(Key.Space))
                RaiseDisablePressed();
            if (keys.Contains(Key.LeftAlt) && keys.Contains(Key.LeftControl) && keys.Contains(Key.Return))
                RaiseEStopPressed();
        }


        private bool disposed = false;
        public void Dispose()
        {
            if (!disposed)
            {
                disposed = true;
                Stop();
            }
        }
    }
}

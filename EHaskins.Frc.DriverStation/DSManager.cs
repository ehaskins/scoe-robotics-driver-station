using System;
using EHaskins.Utilities;
using System.Collections.ObjectModel;
using EHaskins.Frc.Communication;
using EHaskins.Frc.Communication.SaveState;
using System.Linq;
using System.IO;
using System.Windows;
using System.Xml.Linq;
namespace EHaskins.Frc.Communication.DriverStation
{
    public class DSManager : NotificationObject, IDisposable
    {
        private KeyboardMonitor _KeyboardMonitor;
        public KeyboardMonitor KeyboardMonitor
        {
            get { return _KeyboardMonitor; }
            set
            {
                if (_KeyboardMonitor == value)
                    return;
                _KeyboardMonitor = value;
                RaisePropertyChanged("KeyboardMonitor");
            }
        }

        private DSTimer _Timer;
        public DSTimer Timer
        {
            get { return _Timer; }
            set
            {
                if (_Timer == value)
                    return;
                _Timer = value;
                RaisePropertyChanged("Timer");
            }
        }

        protected ObservableCollection<DriverStation> InnerDriverStations { get; set; }
        public ReadOnlyObservableCollection<DriverStation> DriverStations { get; protected set; }

        public JoystickManager JoystickManager { get; set; }

        public void Start()
        {
            InnerDriverStations = new ObservableCollection<DriverStation>();
            DriverStations = new ReadOnlyObservableCollection<DriverStation>(InnerDriverStations);
            KeyboardMonitor = new KeyboardMonitor();
            KeyboardMonitor.Start();
            Timer = new DSMicroTimer();
            JoystickManager = new JoystickManager();
        }
        public void Stop()
        {
            Timer.Dispose();
            KeyboardMonitor.Stop();
            JoystickManager.Dispose();
        }
        public void AddDriverStation()
        {
            AddDriverStation(new DriverStation() { Connection = new UdpTransmitter() });
        }
        public void AddDriverStation(DriverStation ds)
        {
            Timer.AddDriverStation(ds);
            KeyboardMonitor.AddDriverStation(ds);
            InnerDriverStations.Add(ds);
        }
        private DriverStation GetNewDs()
        {

            var Joysticks = JoystickManager.Joysticks;
            var ds = new DriverStation() { TeamNumber = 6, Connection = new UdpTransmitter() { ReceivePort = 1150, TransmitPort = 1110 } };


            for (int i = 0; i < 4; i++)
            {
                var stick = i < Joysticks.Length ? new SlimDXJoystick(JoystickManager, Joysticks[i].Information.InstanceName) : new SlimDXJoystick(JoystickManager, "");
                stick.JoystickNumber = i;
                ds.Joysticks[i] = stick;
            }

            return ds;
        }
        public void LoadState(string path)
        {
            try
            {
                var data = XDocument.Parse(File.ReadAllText(path));
                var i = 0;
                var ds = data.Element("DriverStation");
                var dsstates = ds.Elements("DSState").ToArray();

                foreach (var dsdata in dsstates)
                {
                    if (DriverStations.Count <= i)
                    {
                        AddDriverStation(GetNewDs());
                    }

                    DriverStations[i].LoadState(dsdata, JoystickManager);
                    i++;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading state: " + ex.Message);
            }
        }
        public void SaveState(string path)
        {
            try
            {
                var data = new XElement("DriverStation");
                data.Add((from ds in InnerDriverStations select ds.GetState()));
                var doc = new XDocument();
                doc.Add(data);
                File.WriteAllText(path, doc.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving state: " + ex.Message);
            }
        }

        bool _disposed;
        public void Dispose()
        {
            if (!_disposed)
                Stop();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using EHaskins.Frc.Communication;
using System.Net;
using System.ComponentModel;
using EHaskins.Frc.Communication.DriverStation;
using SlimDX.DirectInput;
using System.Windows;

namespace EHaskins.Frc.DriverStation
{
    public class DriverStationVM 
    {
        public JoystickManager JoystickManager {get; set;}


        public DriverStationVM()
        {
            JoystickManager = new JoystickManager();
            Configuration.UserControlDataSize = 104;
            Configuration.UserStatusDataSize = 152;
            DriverStations = new ObservableCollection<Communication.DriverStation.DriverStation>();

            //var ds = new Communication.DriverStation.DriverStation() { TeamNumber = 1692, Connection = new UdpTransmitter() { Network = 172, Host = 198 } };
            //var ds = new Communication.DriverStation.DriverStation() { TeamNumber = 0, Connection = new UdpTransmitter() { Network = 127, Host = 1 } };
            var ds = new Communication.DriverStation.DriverStation() { TeamNumber = 1103, Connection = new UdpTransmitter() { Network = 10, Host = 2, ReceivePort=1150, TransmitPort=1110 } };
            ds.Started += this.DSStarted;
            ds.Start();
            DriverStations.Add(ds);

            var ds2 = new Communication.DriverStation.DriverStation { TeamNumber = 1103, Connection = new UdpTransmitter() { TransmitPort = 2110, ReceivePort = 2150 } };
            ds2.Started += this.DSStarted;
            //ds2.Start();
            DriverStations.Add(ds2);
        }

        private void DSStarted(object sender, EventArgs e)
        {
            var ds = sender as Communication.DriverStation.DriverStation;

            var Joysticks = JoystickManager.Joysticks;

            for (int i = 0; i < 4; i++)
            {
                var stick = i < Joysticks.Length ? new SlimDXJoystick(JoystickManager, Joysticks[i].Information.InstanceName, true) : new SlimDXJoystick(JoystickManager, "", true);
                stick.JoystickNumber = i;
                ds.ControlData.Joysticks[i] = stick;
            }
        }

        public ObservableCollection<Communication.DriverStation.DriverStation> DriverStations { get; set; }

        public void WindowClosing(object sender, CancelEventArgs e)
        {
            foreach (var ds in DriverStations)
            {
                if (ds != null)
                {
                    if (ds.IsEnabled)
                        ds.Stop();
                    ds.Dispose();
                }
            }
            JoystickManager.Dispose();
        }

    }
}

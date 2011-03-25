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

namespace EHaskins.Frc.DS
{
    public class DriverStationVM 
    {
        public JoystickManager JoystickManager {get; set;}

        public DriverStationVM()
        {
            JoystickManager = new JoystickManager();
            Configuration.UserControlDataSize = 64;
            Configuration.UserStatusDataSize = 64;
            DriverStations = new ObservableCollection<DriverStation>();

            var ds = new DriverStation() { TeamNumber = 1692, Network = 172, HostNumber = 198 };
            ds.Started += this.DSStarted;
            ds.Start();
            DriverStations.Add(ds);

            var ds2 = new DriverStation { TeamNumber = 1103, TransmitPort = 2110, ReceivePort = 2150 };
            ds2.Started += this.DSStarted;
            ds2.Start();
            DriverStations.Add(ds2);
        }

        private void DSStarted(object sender, EventArgs e)
        {
            var ds = sender as DriverStation;

            var Joysticks = JoystickManager.Joysticks;

            for (int i = 0; i < 4; i++)
            {
                var stick = i < Joysticks.Length ? new SlimDxJoystick(JoystickManager, Joysticks[i].Information.InstanceName) : new SlimDxJoystick(JoystickManager, "");

                ds.ControlData.Joysticks[i] = stick;
            }
        }

        public ObservableCollection<DriverStation> DriverStations { get; set; }

        public void WindowClosing(object sender, CancelEventArgs e)
        {
            foreach (var driverStation in DriverStations)
            {
                driverStation.Stop();
                driverStation.Dispose();
            }
            JoystickManager.Dispose();
        }

    }
}

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
            var Joysticks = JoystickManager.Joysticks;
            Configuration.UserControlDataSize = 64;
            Configuration.UserStatusDataSize = 64;
            DriverStations = new ObservableCollection<DriverStation>();
            var ds = new DriverStation(1692);
            for (int i = 0; i < 4; i++)
            {
                var stick = i < Joysticks.Length ? new SlimDxJoystick(JoystickManager, Joysticks[i].Information.InstanceName) : new SlimDxJoystick(JoystickManager, "");

                ds.ControlData.Joysticks[i] = stick;
            }

            ds.Open(1692, new IPEndPoint(IPAddress.Parse("172.16.92.198"), ds.TransmitPort));

            DriverStations.Add(ds);

            /*DriverStation ds2 = new DriverStation(1103);
            ds2.TransmitPort = 2110;
            ds2.ReceivePort = 2150;
            ds2.Open(1103);
            DriverStations.Add(ds2);*/
        }

        public ObservableCollection<DriverStation> DriverStations { get; set; }

        public void WindowClosing(object sender, CancelEventArgs e)
        {
            foreach (DriverStation driverStation in DriverStations)
            {
                driverStation.Close();
                driverStation.Dispose();
            }

        }

    }
}

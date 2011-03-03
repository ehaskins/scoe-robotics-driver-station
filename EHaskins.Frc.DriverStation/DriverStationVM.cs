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
        public DriverStationVM()
        {
            Configuration.UserControlDataSize = 64;
            Configuration.UserStatusDataSize = 64;
            DriverStations = new ObservableCollection<DriverStation>();
            var ds = new SlimDXDriverStation(1692, GetJoysticks());
            ds.Open(1692, new IPEndPoint(IPAddress.Parse("172.16.92.198"), ds.TransmitPort));

            DriverStations.Add(ds);
            DriverStation ds2 = new DriverStation(1103);
            ds2.TransmitPort = 2110;
            ds2.ReceivePort = 2150;
            ds2.Open(1103);
            DriverStations.Add(ds2);
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

        private SlimDX.DirectInput.Joystick[] GetJoysticks()
        {
            var dinput = new DirectInput();
            List<SlimDX.DirectInput.Joystick> sticks = new List<SlimDX.DirectInput.Joystick>();
            foreach (DeviceInstance device in dinput.GetDevices(DeviceClass.GameController, DeviceEnumerationFlags.AttachedOnly))
            {
                // create the device
                try
                {
                    var stick = new SlimDX.DirectInput.Joystick(dinput, device.InstanceGuid);
                    stick.Acquire();

                    foreach (DeviceObjectInstance deviceObject in stick.GetObjects())
                    {
                        if ((deviceObject.ObjectType & ObjectDeviceType.Axis) != 0)
                            stick.GetObjectPropertiesById((int)deviceObject.ObjectType).SetRange(-1000, 1000);
                    }

                    sticks.Add(stick);
                }
                catch (DirectInputException)
                {
                }
            }

            foreach (var stick in sticks)
            {
                Console.WriteLine(stick.Information.InstanceName);
                /*foreach (DeviceObjectInstance deviceObject in stick.GetObjects())
                {
                    if ((deviceObject.ObjectType & ObjectDeviceType.Axis) != 0)
                        stick.GetObjectPropertiesById((int)deviceObject.ObjectType).SetRange(-1000, 1000);
                }

                // acquire the device
                stick.Acquire();*/
            }
            Console.WriteLine("Found " + sticks.Count() + " sticks.");

            return sticks.ToArray();
        }
    }
}

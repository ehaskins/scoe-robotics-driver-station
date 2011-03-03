using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EHaskins.Frc.Communication;
using System.Net;
using SlimDX;
using SlimDX.DirectInput;
using EHaskins.Frc.Communication.DriverStation;

namespace EHaskins.Frc.DriverStationCli
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(Crc32.Compute(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 0 }));
            var app = new VirtualDSCli();
            app.Run();
        }
    }

    class VirtualDSCli
    {
        DriverStation ds;
        DriverStation ds2;
        DirectInput dinput;
        List<SlimDX.DirectInput.Joystick> sticks;
        public void Run()
        {
            dinput = new DirectInput();
            sticks = new List<SlimDX.DirectInput.Joystick>();
            Communication.Configuration.UserControlDataSize = 64;
            Communication.Configuration.UserStatusDataSize = 64;
            ds = new DriverStation(1692);

            ds.NewDataReceived += NewDataReceived;
            ds.SendingData += SendingData;
            ds.TransmitPort = 1110;
            ds.Open(1692, new IPEndPoint(IPAddress.Parse("172.16.92.198"), ds.TransmitPort));

            //ds2 = new VirtualDS(1103);
            //ds2.TransmitPort = 1240;
            //ds2.ReceivePort = 1250;
            //ds2.Open(1103, new IPEndPoint(IPAddress.Parse("127.0.0.1"), 1240));
            ds2 = new DriverStation(1692);
            ds2.TransmitPort = 1240;
            ds2.ReceivePort = 1250;
            //ds2.Open(1692, new IPEndPoint(IPAddress.Parse("172.16.92.199"), 1240));
            //ds.Open(1103);
            while (true)
            {
                var key = Console.ReadKey();

                switch (key.Key)
                {
                    case ConsoleKey.E:
                        Console.WriteLine("Enabled");
                        ds.ControlData.Mode.IsEnabled = true;
                        ds.ControlData.Mode.IsEStop = false;

                        ds2.ControlData.Mode.IsEnabled = true;
                        ds2.ControlData.Mode.IsEStop = false;
                        break;
                    case ConsoleKey.D:
                        Console.WriteLine("Disabled");
                        ds.ControlData.Mode.IsEnabled = false;
                        ds.ControlData.Mode.IsEStop = false;

                        ds2.ControlData.Mode.IsEnabled = false;
                        ds2.ControlData.Mode.IsEStop = false;
                        break;
                    case ConsoleKey.Spacebar:
                        Console.WriteLine("E-Stop");
                        ds.ControlData.Mode.IsEnabled = false;
                        ds.ControlData.Mode.IsEStop = true;

                        ds2.ControlData.Mode.IsEnabled = false;
                        ds2.ControlData.Mode.IsEStop = true;
                        break;
                    case ConsoleKey.T:
                        ds.ControlData.Mode.IsAutonomous = false;

                        ds2.ControlData.Mode.IsAutonomous = false;
                        break;
                    case ConsoleKey.A:
                        ds.ControlData.Mode.IsAutonomous = true;

                        ds2.ControlData.Mode.IsAutonomous = true;
                        break;
                    case ConsoleKey.J:
                        UpdateSticks();
                        break;
                    case ConsoleKey.Q:
                        ds.Close();
                        return;
                }


            }
        }


        private void SendingData(object sender, EventArgs e)
        {
            var dataSticks = ds.ControlData.Joysticks;
            for (int i = 0; i < 4; i++)
            {
            
                if (sticks.Count > i)
                {
                    var stick = sticks[i];
                    var dataStick = dataSticks[i];
                    if (stick.Acquire().IsSuccess && stick.Poll().IsSuccess)
                    {
                        var state = stick.GetCurrentState();
                        if (Result.Last.IsSuccess)
                        {
                            dataStick.Axes[0] = state.X / 1000f;
                            dataStick.Axes[1] = state.Y / 1000f;
                            dataStick.Axes[2] = state.Z / 1000f;
                            dataStick.Axes[3] = state.RotationX / 1000f;
                            dataStick.Axes[4] = state.RotationY / 1000f;
                            dataStick.Axes[5] = state.RotationZ / 1000f;
                        }

                        var buttons = state.GetButtons();
                        for (int button = 0; button < buttons.Length; button++)
                        {
                            dataStick.Buttons[button] = buttons[button];
                        }
                    }
                }
            }

            //string axes = "";
            //foreach (var axis in dataSticks[0].Axes){
            //    axes += " " + axis.ToString("f2");
            //}
            //Console.WriteLine(axes);
        }
        public void NewDataReceived(object sender, EventArgs e)
        {
            try
            {
                string mode = "";
                if (ds.StatusData != null && ds.IsSyncronized)
                {
                    if (ds.StatusData.Mode.IsEStop)
                    {
                        mode = "E-Stop";
                    }
                    else
                    {
                        mode = ds.StatusData.Mode.IsEnabled ? "Enabled" : "Disabled";
                    }
                }
                else
                {
                    mode = "Not Connected";
                }
                Console.WriteLine(String.Format("Packet# {0}, {1}, Resync: {2}", ds.ControlData.PacketId, mode, ds.ControlData.Mode.IsResync));
            }
            catch (Exception ex)
            {
            }
        }
        private void UpdateSticks()
        {
            for (int i = sticks.Count - 1; i >= 0; i--)
            {
                if (sticks[i] != null)
                {
                    sticks[i].Unacquire();
                    sticks[i].Dispose();
                    sticks.Remove(sticks[i]);
                }
            }
            int stickCount = 0;
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
        }
    }
}

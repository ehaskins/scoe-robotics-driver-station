using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EHaskins.Frc.Dashboard;
using EHaskins.Frc.Communication;
using System.Net;
namespace EHaskins.Frc.DriverStationCli
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(Crc32.Compute(new byte[] {1, 2, 3, 4, 5, 6, 7, 8, 9, 0}));
            var app = new VirtualDSCli();
            app.Run();
        }
    }

    class VirtualDSCli
    {
        VirtualDS ds;
        VirtualDS ds2;
        public void Run()
        {
            Configuration.UserControlDataSize = 64;
            Configuration.UserStatusDataSize = 64;
            ds = new VirtualDS(1692);

            ds.NewDataReceived += NewDataReceived;
            ds.Open(1692, new IPEndPoint(IPAddress.Parse("172.16.92.198"), 1140));

            //ds2 = new VirtualDS(1103);
            //ds2.TransmitPort = 1240;
            //ds2.ReceivePort = 1250;
            //ds2.Open(1103, new IPEndPoint(IPAddress.Parse("127.0.0.1"), 1240));
            ds2 = new VirtualDS(1692);
            ds2.TransmitPort = 1240;
            ds2.ReceivePort = 1250;
            //ds2.Open(1692, new IPEndPoint(IPAddress.Parse("172.16.92.199"), 1240));
            //ds.Open(1103);
            while (true)
            {
                var key = Console.ReadKey();

                if (key.Key == ConsoleKey.E)
                {

                    Console.WriteLine("Enabled");
                    ds.CommandData.Mode.Enabled = true;
                    ds.CommandData.Mode.Autonomous = true;
                    ds.CommandData.Mode.EStop = false;

                    ds2.CommandData.Mode.Enabled = true;
                    ds2.CommandData.Mode.Autonomous = true;
                    ds2.CommandData.Mode.EStop = false;
                }
                else if (key.Key == ConsoleKey.D)
                {
                    Console.WriteLine("Disabled");
                    ds.CommandData.Mode.Enabled = false;
                    ds.CommandData.Mode.Autonomous = true;
                    ds.CommandData.Mode.EStop = false;

                    ds2.CommandData.Mode.Enabled = false;
                    ds2.CommandData.Mode.Autonomous = true;
                    ds2.CommandData.Mode.EStop = false;
                }     
                else if (key.Key == ConsoleKey.Spacebar)
                {
                    Console.WriteLine("E-Stop");
                    ds.CommandData.Mode.Enabled = false;
                    ds.CommandData.Mode.Autonomous = true;
                    ds.CommandData.Mode.EStop = true;

                    ds2.CommandData.Mode.Enabled = false;
                    ds2.CommandData.Mode.Autonomous = true;
                    ds2.CommandData.Mode.EStop = true;
                }
                else if (key.Key == ConsoleKey.Q)
                {
                    ds.Close();
                    return;
                }
            }
        }

        public void NewDataReceived(object sender, EventArgs e)
        {
            try
            {
                var stick = ds.CommandData.Joysticks[1];
                stick.Axes[1] = stick.Axes[1] > 1 ? -1 : stick.Axes[1] + 0.1;

                string mode = "";
                if (ds.RobotStatus != null && ds.IsSyncronized)
                {
                    if (ds.RobotStatus.Mode.EStop)
                    {
                        mode = "E-Stop";
                    }
                    else
                    {
                        mode = ds.RobotStatus.Mode.Enabled ? "Enabled" : "Disabled";
                    }
                }
                else
                {
                    mode = "Not Connected";
                }
                Console.WriteLine(String.Format("Packet# {0}, {1}, Resync: {2}", ds.CommandData.PacketId, mode, ds.CommandData.Mode.Resync));
            }
            catch (Exception ex)
            {
            }
        }
    }
}

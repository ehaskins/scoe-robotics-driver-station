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
            var app = new VirtualDSCli();
            app.Run();
        }
    }

    class VirtualDSCli
    {
        VirtualDS ds;
        public void Run()
        {
            Configuration.UserControlDataSize = 64;
            Configuration.UserStatusDataSize = 64;
            ds = new VirtualDS(1103);
            ds.NewDataReceived += NewDataReceived;
            //ds.Open(1103, new IPEndPoint(IPAddress.Parse("127.0.0.1"), 1110));
            ds.Open(1692, new IPEndPoint(IPAddress.Parse("172.16.92.198"), 1140));
            //ds.Open(1103, new IPEndPoint(IPAddress.Parse("172.11.3.198"), 1140));
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
                }
                else if (key.Key == ConsoleKey.D)
                {
                    Console.WriteLine("Disabled");
                    ds.CommandData.Mode.Enabled = false;
                    ds.CommandData.Mode.Autonomous = true;
                    ds.CommandData.Mode.EStop = false;
                }
                else if (key.Key == ConsoleKey.Spacebar)
                {
                    Console.WriteLine("E-Stop");
                    ds.CommandData.Mode.Enabled = false;
                    ds.CommandData.Mode.Autonomous = true;
                    ds.CommandData.Mode.EStop = true;
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
                string mode = "";
                if (ds.RobotStatus != null)
                {
                    if (ds.CommandData.Mode.EStop)
                    {
                        mode = "E-Stop";
                    }
                    else
                    {
                        mode = ds.CommandData.Mode.Enabled ? "Enabled" : "Disabled";
                    }
                }
                else
                {
                    mode = "no status";
                }
                Console.WriteLine(String.Format("Packet# {0}, {1}, Resync: {2}", ds.CommandData.PacketId, mode, ds.CommandData.Mode.Resync));
            }
            catch (Exception)
            {
            }
        }
    }
}

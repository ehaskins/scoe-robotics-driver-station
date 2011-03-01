using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            ds.SendingData += SendingData;
            ds.TransmitPort = 1110;
            ds.Open(1692, new IPEndPoint(IPAddress.Parse("172.16.92.198"), ds.TransmitPort));

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
                    ds.ControlData.Mode.Enabled = true;
                    ds.ControlData.Mode.Autonomous = true;
                    ds.ControlData.Mode.EStop = false;

                    ds2.ControlData.Mode.Enabled = true;
                    ds2.ControlData.Mode.Autonomous = true;
                    ds2.ControlData.Mode.EStop = false;
                }
                else if (key.Key == ConsoleKey.D)
                {
                    Console.WriteLine("Disabled");
                    ds.ControlData.Mode.Enabled = false;
                    ds.ControlData.Mode.Autonomous = true;
                    ds.ControlData.Mode.EStop = false;

                    ds2.ControlData.Mode.Enabled = false;
                    ds2.ControlData.Mode.Autonomous = true;
                    ds2.ControlData.Mode.EStop = false;
                }     
                else if (key.Key == ConsoleKey.Spacebar)
                {
                    Console.WriteLine("E-Stop");
                    ds.ControlData.Mode.Enabled = false;
                    ds.ControlData.Mode.Autonomous = true;
                    ds.ControlData.Mode.EStop = true;

                    ds2.ControlData.Mode.Enabled = false;
                    ds2.ControlData.Mode.Autonomous = true;
                    ds2.ControlData.Mode.EStop = true;
                }
                else if (key.Key == ConsoleKey.Q)
                {
                    ds.Close();
                    return;
                }
            }
        }

        private void SendingData(object sender, EventArgs e)
        {
            
        }
        public void NewDataReceived(object sender, EventArgs e)
        {
            try
            {
                string mode = "";
                if (ds.StatusData != null && ds.IsSyncronized)
                {
                    if (ds.StatusData.Mode.EStop)
                    {
                        mode = "E-Stop";
                    }
                    else
                    {
                        mode = ds.StatusData.Mode.Enabled ? "Enabled" : "Disabled";
                    }
                }
                else
                {
                    mode = "Not Connected";
                }
                Console.WriteLine(String.Format("Packet# {0}, {1}, Resync: {2}", ds.ControlData.PacketId, mode, ds.ControlData.Mode.Resync));
            }
            catch (Exception ex)
            {
            }
        }
    }
}

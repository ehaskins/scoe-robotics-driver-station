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
            VirtualDS ds = new VirtualDS(1103);
            //ds.Open(1103, new IPEndPoint(IPAddress.Parse("127.0.0.1"), 1110));
            ds.Open(1692, new IPEndPoint(IPAddress.Parse("172.16.92.198"), 1140));
            //ds.Open(1103);
            Console.WriteLine("Enabled");
            ds.CommandData.Mode.Enabled = true;
            ds.CommandData.Mode.Autonomous = true;
            ds.CommandData.Mode.EStop = false;
            Console.ReadLine();
            Console.WriteLine("Disabled");
            ds.CommandData.Mode.Enabled = false;
            ds.CommandData.Mode.Autonomous = true;
            ds.CommandData.Mode.EStop = false;
            Console.ReadLine();
            Console.WriteLine("E-Stop");
            ds.CommandData.Mode.Enabled = false;
            ds.CommandData.Mode.Autonomous = true;
            ds.CommandData.Mode.EStop = true;
            Console.ReadLine();
            Console.WriteLine("Enabled");
            ds.CommandData.Mode.Enabled = true;
            ds.CommandData.Mode.Autonomous = true;
            ds.CommandData.Mode.EStop = false;
            Console.ReadLine();
            ds.Close();
        }
    }
}

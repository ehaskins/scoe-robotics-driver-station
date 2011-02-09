using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EHaskins.Frc.Dashboard;
using System.Net;
namespace EHaskins.Frc.DriverStationCli
{
    class Program
    {
        static void Main(string[] args)
        {
            VirtualDS ds = new VirtualDS(1103);
            //ds.Open(1103, new IPEndPoint(IPAddress.Parse("127.0.0.1"), 1110));
            ds.Open(1103);
            ds.ControlData.ControlData.Enabled = true;
            ds.ControlData.ControlData.Autonomous = true;
            Console.ReadLine();
            ds.ControlData.ControlData.Enabled = false;
            ds.ControlData.ControlData.Autonomous = true;
            Console.ReadLine();
        }
    }
}

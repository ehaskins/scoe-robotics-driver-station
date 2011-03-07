using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EHaskins.Frc.Communication;

namespace EHaskins.Frc.RobotEmulatorCLI
{
    class Program
    {
        static void Main(string[] args)
        {
            var app = new RobotTestApp();
            app.Run();
        }


    }

    class RobotTestApp
    {
        VirtualRobot vRobot;
        public void Run()
        {
            vRobot = new VirtualRobot(1103);
            vRobot.ReceivePort = 1240;
            vRobot.TransmitPort = 1250;
            vRobot.Start();
            vRobot.StatusData.BatteryVoltage = 11.03;
            vRobot.NewDataReceived += NewDataReceived;

            Console.WriteLine("vRobot Started");
            Console.ReadLine();
            vRobot.StatusData.CodeRunning = true;

            Console.ReadLine();
        }

        private void NewDataReceived(object sender, EventArgs e)
        {
            string mode = "";
            if (vRobot.CommandData.Mode.IsEStop)
            {
                mode = "E-Stop";
            }
            else
            {
                mode = vRobot.CommandData.Mode.IsEnabled ? "Enabled" : "Disabled";
            }
            Console.WriteLine(String.Format("Packet# {0}, {1}, Resync: {2}", vRobot.StatusData.ReplyId, mode, vRobot.CommandData.Mode.IsResync));
        }

    }
}

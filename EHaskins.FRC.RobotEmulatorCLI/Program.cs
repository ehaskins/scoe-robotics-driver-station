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
        Robot vRobot;
        public void Run()
        {
            vRobot = new Robot(9245);
            vRobot.UserControlDataLength = 64;
            vRobot.UserStatusDataLength = 64;
            vRobot.ReceivePort = 1110;
            vRobot.TransmitPort = 1120;
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
            if (vRobot.ControlData.Mode.IsEStop)
            {
                mode = "E-Stop";
            }
            else
            {
                mode = vRobot.ControlData.Mode.IsEnabled ? "Enabled" : "Disabled";
            }
            Console.WriteLine(String.Format("Packet# {0}, {1}, Resync: {2}", vRobot.StatusData.ReplyId, mode, vRobot.ControlData.Mode.IsResync));
        }

    }
}

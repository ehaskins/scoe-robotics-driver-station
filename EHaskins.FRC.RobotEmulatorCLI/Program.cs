using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EHaskins.Frc.Dashboard;

namespace EHaskins.FRC.RobotEmulatorCLI
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
            vRobot.Status.BatteryVoltage = 11.03;
            vRobot.NewDataReceived += NewDataReceived;

            Console.WriteLine("vRobot Started");
            Console.ReadLine();
            vRobot.Status.CodeRunning = true;

            Console.ReadLine();
        }

        private void NewDataReceived(object sender, EventArgs e)
        {
            string mode = "";
            if (vRobot.Status.ControlData.EStop){
                mode = "E-Stop";
            }
            else
            {
                mode = vRobot.Status.ControlData.Enabled ? "Enabled" : "Disabled";
            }
            Console.WriteLine(String.Format("Packet# {0}, {1}", vRobot.Status.ReplyId, mode));
        }

    }
}

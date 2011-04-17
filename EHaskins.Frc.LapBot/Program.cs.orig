using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EHaskins.Frc.Communication;
using Phidgets;
using EHaskins.Utilities.NumericExtensions;

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
        Servo servoController;
        VirtualRobot vRobot;
        public void Run()
        {
            servoController = new Servo();
            servoController.Attach += this.ServoAttached;
            vRobot = new VirtualRobot(9245);
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
            ControlData control = vRobot.ControlData;
            if (control.Mode.IsEStop)
            {
                mode = "E-Stop";
            }
            else
            {
                mode = control.Mode.IsEnabled ? "Enabled" : "Disabled";
            }
            Console.WriteLine(String.Format("Packet# {0}, {1}, Resync: {2}", vRobot.StatusData.ReplyId, mode, control.Mode.IsResync));


            if (servoController.Attached)
            {
                var camX = control.Joysticks[0].Axes[2];
                var camY = control.Joysticks[0].Axes[4];
                var x = control.Joysticks[0].Axes[0];
                var y = control.Joysticks[0].Axes[1];
                var z = control.Joysticks[0].Axes[3];


                var nw = y + x + z;
                var ne = y - x - z;
                var sw = y + x - z;
                var se = y - x + z;

                nw = nw.Limit(-1, 1);
                ne = ne.Limit(-1, 1);
                sw = sw.Limit(-1, 1);
                se = se.Limit(-1, 1);

                servoController.servos[0].Position = nw + 1;
                servoController.servos[1].Position = sw + 1;
                servoController.servos[2].Position = ne + 1;
                servoController.servos[3].Position = se + 1;
                servoController.servos[4].Position = camX + 1;
                servoController.servos[5].Position = camY + 1;
            }
        }
        private void ServoAttached(object sender, Phidgets.Events.AttachEventArgs e)
        {
            for (int i = 0; i < 8; i++)
            {
                servoController.servos[i].setServoParameters(0.9, 2.1, 2);
            }
        }

    }
}

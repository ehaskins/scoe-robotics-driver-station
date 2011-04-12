using System;
using EHaskins.Frc.Communication;
using Phidgets;
using EHaskins.Utilities.NumericExtensions;
using System.Diagnostics;

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
        Robot robot;
        public void Run()
        {
            servoController = new Servo();
            servoController.open();
            robot = new Robot(1692);
            robot.UserControlDataLength = 64;
            robot.UserStatusDataLength = 64;
            robot.Connection = new UdpTransmitter() { ReceivePort = 1110, TransmitPort = 1150, PacketSize = 155, IsResponderMode = true };
            robot.Start();
            robot.StatusData.BatteryVoltage = 11.03;
            robot.NewDataReceived += NewDataReceived;

            Debug.WriteLine("vRobot Started");
            robot.StatusData.CodeRunning = true;
            Console.ReadLine();
            robot.StatusData.CodeRunning = true;

            Console.ReadLine();

            robot.Stop();
        }

        double camX = 0;
        double camY = 0;
        private void NewDataReceived(object sender, EventArgs e)
        {
            string mode = "";
            ControlData control = robot.ControlData;
            if (control.Mode.IsEStop)
            {
                mode = "E-Stop";
            }
            else
            {
                mode = control.Mode.IsEnabled ? "Enabled" : "Disabled";
            }
            Console.WriteLine(String.Format("Packet# {0}, {1}, Resync: {2}", robot.StatusData.ReplyId, mode, control.Mode.IsResync));

            if (control.Mode.IsEnabled)
            {
                if (control.Mode.IsAutonomous)
                    AutonomousPeriodic();
                else
                    TeleopPeriodic();
            }
            else
            {
                DisabledPeriodic();
            }
            
        }
        private void ServoAttached(object sender, Phidgets.Events.AttachEventArgs e)
        {
            for (int i = 0; i < 8; i++)
            {
                servoController.servos[i].setServoParameters(0.9, 2.1, 2);
            }
        }

        private void TeleopPeriodic()
        {
            ControlData control = robot.ControlData;
            Console.WriteLine("Servo attached?" + servoController.Attached);
            if (servoController.Attached)
            {
                for (int i = 0; i < 6; i++)
                {
                    servoController.servos[i].Engaged = true;
                    servoController.servos[i].setServoParameters(0.9, 2.1, 2);
                }
                //var camX = control.Joysticks[0].Axes[2];
                //var camY = control.Joysticks[0].Axes[4];
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

                camX += 0.01;
                camY += 0.01;
                if (camX > -1)
                    camX = 0;
                if (camY > 1)
                    camY = -1;
            }
        }

        private void AutonomousPeriodic()
        {
            
        }
        private void DisabledPeriodic()
        {
            if (servoController.Attached)
            {
                for (int i = 0; i < 8; i++)
                {
                    servoController.servos[i].Engaged = false;
                }
            }
        }
    }
}

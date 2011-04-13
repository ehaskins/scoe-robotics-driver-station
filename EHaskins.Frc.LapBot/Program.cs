using System;
using EHaskins.Frc.Communication;
using Phidgets;
using EHaskins.Utilities.NumericExtensions;
using System.Diagnostics;
using EHaskins.Frc.Communication.Robot;

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
        AdvancedServo servoController;
        Robot robot;
        public void Run()
        {
            Console.WriteLine("Starting servocontroller...");
            servoController = new AdvancedServo();
            servoController.open();
            servoController.waitForAttachment();
            Console.WriteLine("Starting robot...");
            robot = new Robot(9246) { UserControlDataLength = 64, 
                        UserStatusDataLength = 64, 
                        Connection = new UdpTransmitter { ReceivePort = 1110, TransmitPort = 1150, PacketSize = 155, IsResponderMode = true } };
            robot.Connected += new EventHandler(RobotConnected);
            robot.Disconnected += new EventHandler(RobotDisconnected);
            robot.Start();
            robot.StatusData.BatteryVoltage = 11.03;
            robot.NewDataReceived += NewDataReceived;

            Debug.WriteLine("vRobot Started");
            robot.StatusData.CodeRunning = true;
            //Console.ReadLine();
            //robot.StatusData.CodeRunning = true;
            Console.ReadLine();


            robot.Stop();
            servoController.close();
        }

        private void RobotDisconnected(object sender, EventArgs e)
        {
            SetOutputsEnabled(false);
            Console.WriteLine("Disconnected");
        }
        void RobotConnected(object sender, EventArgs e)
        {
            Console.WriteLine("Connected");
        }

        double camX = 0;
        double camY = 0;
        private void NewDataReceived(object sender, EventArgs e)
        {

            ControlData control = robot.ControlData;
            //
            //string mode = "";
            //if (control.Mode.IsEStop)
            //{
            //    mode = "E-Stop";
            //}
            //else
            //{
            //    mode = control.Mode.IsEnabled ? "Enabled" : "Disabled";
            //}
            //Console.WriteLine(String.Format("Packet# {0}, {1}, Resync: {2}", robot.StatusData.ReplyId, mode, control.Mode.IsResync));

            if (control.Mode.IsEnabled)
            {
                SetOutputsEnabled(true);
                if (control.Mode.IsAutonomous)
                    AutonomousPeriodic();
                else
                    TeleopPeriodic();
            }
            else
            {
                SetOutputsEnabled(false);
                DisabledPeriodic();
            }
            
        }
        private void ServoAttached(object sender, Phidgets.Events.AttachEventArgs e)
        {
        }

        private bool enabledValue = true;
        private void SetOutputsEnabled(bool enabled)
        {
            if (servoController.Attached && enabled && !enabledValue)
            {
                for (int i = 0; i < 8; i++)
                {
                    servoController.servos[i].Engaged = true;
                    servoController.servos[i].setServoParameters(600, 2350, 2, 2);
                    servoController.servos[i].SpeedRamping = false;

                    enabledValue = true;
                }
            }
            else if (servoController.Attached && !enabled && enabledValue)
            {
                for (int i = 0; i < 8; i++)
                {
                    servoController.servos[i].Engaged = false;
                    enabledValue = false;
                }
            }
        }


        private void TeleopPeriodic()
        {
            ControlData control = robot.ControlData;
            try
            {
                if (servoController.Attached)
                {

                    var camX = control.Joysticks[0].Axes[0];
                    var camY = -control.Joysticks[0].Axes[1];
                    var x = control.Joysticks[0].Axes[0];
                    var y = -control.Joysticks[0].Axes[1];
                    var z = -control.Joysticks[0].Axes[3];


                    var nw = y + x + z;
                    var ne = y - x - z;
                    var sw = y + x - z;
                    var se = y - x + z;

                    nw = nw.Limit(-1, 1);
                    ne = ne.Limit(-1, 1);
                    sw = sw.Limit(-1, 1);
                    se = se.Limit(-1, 1);

                    servoController.servos[0].Position = (nw + 1).Limit(0, 2);
                    servoController.servos[1].Position = (sw + 1).Limit(0, 2);
                    servoController.servos[2].Position = (ne + 1).Limit(0, 2);
                    servoController.servos[3].Position = (se + 1).Limit(0, 2);
                    servoController.servos[4].Position = (camX + 1).Limit(0, 2);
                    servoController.servos[5].Position = (camY + 1).Limit(0, 2);
                }
            }
            catch (PhidgetException ex)
            {
                Console.WriteLine("Phidgets error:" + ex.Message);
                //throw;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private void AutonomousPeriodic()
        {
            
        }
        private void DisabledPeriodic()
        {

        }
    }
}

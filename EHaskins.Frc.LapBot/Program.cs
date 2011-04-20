using System;
using EHaskins.Frc.Communication;
using Phidgets;
using EHaskins.Utilities.NumericExtensions;
using System.Diagnostics;
using EHaskins.Frc.Communication.Robot;
using System.Threading;

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
        Thread phidgetsThread;
        AdvancedServo servoController;
        Robot robot;
        bool isRunning = false;
        public void Run()
        {
            isRunning = true;
            Console.WriteLine("Starting servocontroller...");
            servoController = new AdvancedServo();
            servoController.open();
            servoController.waitForAttachment();
            for (int i = 0; i < 8; i++)
            {
                servoController.servos[i].setServoParameters(900, 2100, 2, 2);
                servoController.servos[i].SpeedRamping = false;
            }
            phidgetsThread = new Thread((ThreadStart)PhidgetsLoop);
            phidgetsThread.Start();
            Console.WriteLine("Starting robot...");
            robot = new Robot(1103)
            {
                UserControlDataLength = 64,
                UserStatusDataLength = 64,
                Connection = new UdpTransmitter { ReceivePort = 1110, TransmitPort = 1150, PacketSize = 155, IsResponderMode = true }
            };
            robot.Connected += new EventHandler(RobotConnected);
            robot.Disconnected += new EventHandler(RobotDisconnected);
            robot.Start();
            robot.StatusData.BatteryVoltage = 11.03;
            robot.NewDataReceived += new EventHandler(NewDataReceived);

            Debug.WriteLine("vRobot Started");
            robot.StatusData.CodeRunning = true;
            //Console.ReadLine();
            //robot.StatusData.CodeRunning = true;
            Console.ReadLine();


            isRunning = false;
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
        }

        bool phidgetsInitComplete = false;
        bool phidgetsDeInitComplete = true;
        private void EnableOutputs()
        {
            if (!phidgetsInitComplete)
            {
                SetOutputsEnabled(true);
                phidgetsInitComplete = true;
                phidgetsDeInitComplete = false;
            }
        }
        private void DisableOutputs()
        {
            if ((!phidgetsDeInitComplete))
            {
                SetOutputsEnabled(false);

                phidgetsInitComplete = false;
                phidgetsDeInitComplete = true;
            }
        }
        private bool enabledValue = false;
        private void SetOutputsEnabled(bool enabled)
        {
            if (servoController.Attached && enabled && !enabledValue)
            {
                for (int i = 0; i < 8; i++)
                {
                    servoController.servos[i].Engaged = true;
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

        private bool CanEabledPhidgets()
        {
            return robot.IsConnected && robot.ControlData.Mode.IsEnabled;
        }
        private void PhidgetsLoop()
        {

            while (isRunning)
            {
                ControlData control = robot.ControlData;
                if (CanEabledPhidgets())
                {
                    EnableOutputs();
                    ushort startPacket = robot.ControlData.PacketId;
                    try
                    {
                        servoController.servos[0].Position = (nw + 1).Limit(0, 2);
                        servoController.servos[1].Position = (sw + 1).Limit(0, 2);
                        servoController.servos[2].Position = (ne + 1).Limit(0, 2);
                        servoController.servos[3].Position = (se + 1).Limit(0, 2);
                        servoController.servos[4].Position = (camX + 1).Limit(0, 2);
                        servoController.servos[5].Position = (camY + 1).Limit(0, 2);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                        DisableOutputs();
                    }
                    if (robot.ControlData.PacketId == startPacket)
                    {
                        //Debug.WriteLine("Spinning");
                        SpinWait.SpinUntil(() => robot.ControlData.PacketId != startPacket);
                    }
                }
                else
                {
                    DisableOutputs();
                    SpinWait.SpinUntil(() => CanEabledPhidgets() || !isRunning);
                }
            }
            DisableOutputs();
        }

        double nw;
        double ne;
        double sw;
        double se;
        double camX = 0;
        double camY = 0;

        private void TeleopPeriodic()
        {
            ControlData control = robot.ControlData;
            try
            {
                camX = control.Joysticks[0].Axes[0];
                camY = -control.Joysticks[0].Axes[1];
                var x = control.Joysticks[0].Axes[0];
                var y = -control.Joysticks[0].Axes[1];
                var z = -control.Joysticks[0].Axes[3];


                nw = y + x + z;
                ne = y - x - z;
                sw = y + x - z;
                se = y - x + z;

                nw = nw.Limit(-1, 1);
                ne = ne.Limit(-1, 1);
                sw = sw.Limit(-1, 1);
                se = se.Limit(-1, 1);
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

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
        Thread startThread;
        AdvancedServo servoController;
        Robot robot;
        public void Run()
        {
            Console.WriteLine("Starting servocontroller...");
            servoController = new AdvancedServo();
            servoController.open();
            servoController.waitForAttachment();
            for (int i = 0; i < 8; i++)
            {
                servoController.servos[i].setServoParameters(600, 2350, 2, 2);
                servoController.servos[i].SpeedRamping = false;
            }
            Console.WriteLine("Starting robot...");
            robot = new Robot(1103) { UserControlDataLength = 64, 
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
                InitPhidgets();
                if (control.Mode.IsAutonomous)
                    AutonomousPeriodic();
                else
                    TeleopPeriodic();
            }
            else
            {
                DeinitPhidgets();
                DisabledPeriodic();
            }
            
        }
        private void ServoAttached(object sender, Phidgets.Events.AttachEventArgs e)
        {
        }

        bool phidgetsInitiaized = false;
        bool phidgetsDeInitiaized = true;
        bool phidgetsInitComplete = false;
        bool phidgetsDeInitComplete = true;
        private void InitPhidgets()
        {
            //if (!phidgetsInitiaized)
            //{
            //    startThread = new Thread((ThreadStart)EnableOutputs);
            //    startThread.Start();
            //    phidgetsInitiaized = true;
            //    phidgetsDeInitiaized = false;
            //}
            EnableOutputs();
        }
        private void DeinitPhidgets(){
            //if (!phidgetsDeInitiaized)
            //{
            //    startThread = new Thread((ThreadStart)DisableOutputs);
            //    startThread.Start();
            //    phidgetsInitiaized = false;
            //    phidgetsDeInitiaized = true;
            //}
            DisableOutputs();
        }
        private void EnableOutputs()
        {
            SetOutputsEnabled(true);
            phidgetsInitComplete = true;
            phidgetsDeInitComplete = false;
        }
        private void DisableOutputs()
        {
            SetOutputsEnabled(false);

            phidgetsInitComplete = false;
            phidgetsDeInitComplete = true;

            startThread = new Thread((ThreadStart)UpdateLoop);
        }
        private bool enabledValue = true;
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

        private void UpdateLoop()
        {
            while (phidgetsInitComplete)
            {
                servoController.servos[0].Position = (nw + 1).Limit(0, 2);
                servoController.servos[1].Position = (sw + 1).Limit(0, 2);
                servoController.servos[2].Position = (ne + 1).Limit(0, 2);
                servoController.servos[3].Position = (se + 1).Limit(0, 2);
                servoController.servos[4].Position = (camX + 1).Limit(0, 2);
                servoController.servos[5].Position = (camY + 1).Limit(0, 2);
            }
        }

        double nw;
        double ne;
        double sw;
        double se;

        private void TeleopPeriodic()
        {
            ControlData control = robot.ControlData;
            try
            {
                if (phidgetsInitComplete)
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

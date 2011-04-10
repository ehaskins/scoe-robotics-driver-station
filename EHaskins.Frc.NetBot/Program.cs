using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware;
using SecretLabs.NETMF.Hardware.NetduinoPlus;
using Microsoft.SPOT.Net.NetworkInformation;
using EHaskins.Frc.Communication;

namespace EHaskins.Frc.NetBot
{
    public class Program
    {
        static String ip = "172.16.92.198";
        static String subnet = "255.255.0.0";
        static String gateway = "0.0.0.0";

        public static void Main()
        {
            int counter = 0;
            var ifaces = NetworkInterface.GetAllNetworkInterfaces();


            if (ifaces[0].IPAddress != ip)
            {
                Debug.Print("Setting IP");
                ifaces[0].EnableStaticIP(ip, subnet, gateway);
            }
            
            Debug.Print("IP:" + ifaces[0].IPAddress + " Mask:" + ifaces[0].SubnetMask);

            var app = new RobotTest();
            app.Run();
            //using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
            //{
            //    socket.Bind(new IPEndPoint(IPAddress.Any, 1110));
            //    var buffer = new byte[1024];
            //    while (true)
            //    {
            //        var count = socket.Receive(buffer, SocketFlags.None);
            //        Debug.Print(count.ToString());
            //    }
            //}
        }

    }

    public class RobotTest
    {
        Robot robot;
        public void Run()
        {
            robot = new Robot(1692);
            robot.UserControlDataLength = 64;
            robot.UserStatusDataLength = 64;
            robot.Connection = new UdpTransmitter() { ReceivePort = 1110, TransmitPort = 1150, PacketSize=155 };
            robot.Start();
            robot.StatusData.BatteryVoltage = 11.03;
            robot.NewDataReceived += NewDataReceived;

            Debug.Print("vRobot Started");
            robot.StatusData.CodeRunning = true;

            while (true)
            {
                Thread.Sleep(1);
            }
        }

        int counter = 0;
        private void NewDataReceived(object sender, EventArgs e)
        {
            counter++;
            string mode = "";
            if (robot.ControlData.Mode.IsEStop)
            {
                mode = "E-Stop";
            }
            else
            {
                mode = robot.ControlData.Mode.IsEnabled ? "Enabled" : "Disabled";
            }
            if (counter % 50 == 0)
                Debug.Print("Packet# " + robot.StatusData.ReplyId +", " + mode.ToString() + ", Resync: " + robot.ControlData.Mode.IsResync);
        }
    }
}

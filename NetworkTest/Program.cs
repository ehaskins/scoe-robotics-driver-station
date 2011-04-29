using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware;
using SecretLabs.NETMF.Hardware.NetduinoPlus;
using Microsoft.SPOT.Net.NetworkInformation;

namespace NetworkTest
{
    public class Program
    {

        static String ip = "172.16.92.198";
        static String subnet = "255.255.0.0";
        static String gateway = "0.0.0.0";
        public static void Main()
        {
            // write your code here
            int counter = 0;
            var ifaces = NetworkInterface.GetAllNetworkInterfaces();


            if (ifaces[0].IPAddress != ip)
            {
                Debug.Print("Setting IP");
                ifaces[0].EnableStaticIP(ip, subnet, gateway);
            }

            Debug.Print("IP:" + ifaces[0].IPAddress + " Mask:" + ifaces[0].SubnetMask);

            using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
            {
                socket.Bind(new IPEndPoint(IPAddress.Any, 1110));
                var buffer = new byte[1024];
                EndPoint ep = new IPEndPoint(IPAddress.Any, 1110);
                while (true)
                {
                    var count = socket.ReceiveFrom(buffer, SocketFlags.None, ref ep);
                    socket.SendTo(buffer, 0, count, SocketFlags.None, ep);
                    Debug.Print(count.ToString());
                }
            }

        }

    }
}

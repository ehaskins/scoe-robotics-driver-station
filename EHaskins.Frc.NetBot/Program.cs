using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware;
using SecretLabs.NETMF.Hardware.NetduinoPlus;
using Microsoft.SPOT.Net.NetworkInformation;

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


            while (true)
            {
                Thread.Sleep(1000);
            }

        }

    }
}

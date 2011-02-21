using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
namespace EHaskins.Next.RobotConfiguratorCli
{
    class Program
    {
        static int configPort = 1000;
        static void Main(string[] args)
        {
            Console.WriteLine("NeXT Arduino Configurator");

            var client = new UdpClient(configPort + 1);

            while (true)
            {
                Console.WriteLine("Press enter to attempt connection.");
                Console.ReadLine();
                var initBytes = new byte[] { 0x01, 0x00, 0x00 };
                var epBroad = new IPEndPoint(IPAddress.Broadcast, configPort);
                client.Send(initBytes, initBytes.Length, epBroad);

                IPEndPoint sourceEp = null;
                var data = client.Receive(ref sourceEp);

                for (int i = 0; i < data.Length; i++)
                {
                    Console.WriteLine(data[i].ToString("x"));
                }
            }
            Console.ReadLine();
        }
    }

    enum Commands
    {
        Discover = 0x01,
        Read = 0x02,
        Write = 0x03,
        Erase = 0x04
    }
}

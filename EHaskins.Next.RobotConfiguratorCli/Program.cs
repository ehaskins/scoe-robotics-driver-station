using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using MiscUtil.IO;
using MiscUtil.Conversion;
using System.IO;
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
                var initBytes = new byte[] { 0x01, 0x00, 0x00};
                var epBroad = new IPEndPoint(IPAddress.Broadcast, configPort);
                client.Send(initBytes, initBytes.Length, epBroad);

                IPEndPoint sourceEp = null;
                var data = client.Receive(ref sourceEp);
                var reader = new EndianBinaryReader(new BigEndianBitConverter(), new MemoryStream(data));
                var id = reader.ReadUInt16();
                Console.WriteLine("Device ID: " + id);

                var mac = reader.ReadBytes(6);
                Console.Write("MAC: ");
                for (int i = 0; i < mac.Length; i++)
                {
                    Console.Write(mac[i].ToString("x"));
                    if (i < 5)
                    {
                        Console.Write(":");
                    }
                    else
                    {
                        Console.WriteLine();
                    }
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

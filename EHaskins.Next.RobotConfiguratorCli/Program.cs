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
        static void Main(string[] args)
        {
            (new Configurator()).Run();
        }
    }

    class Configurator
    {
        List<byte[]> macAddresses = new List<byte[]>();
        int configPort = 1000;
        UdpClient client;
        ushort lastDeviceId;
        public void Run()
        {
            macAddresses.Add(new byte[] { 0x90, 0xA2, 0xDA, 0x00, 0X30, 0x98 });
            macAddresses.Add(new byte[] { 0x90, 0xA2, 0xDA, 0x00, 0X36, 0x2A });
            Console.WriteLine("NeXT Arduino Configurator");

            client = new UdpClient(configPort + 1);
            client.BeginReceive(ReceiveData, null);
            var epBroad = new IPEndPoint(IPAddress.Broadcast, configPort);
            while (true)
            {
                var key = Console.ReadKey();

                Console.WriteLine();
                switch (key.Key)
                {
                    case ConsoleKey.D:
                        SendDiscovery(epBroad);
                        break;
                    case ConsoleKey.W:
                        SendWrite(epBroad);
                        break;
                    case ConsoleKey.E:
                        SendErase(epBroad);
                        break;
                    case ConsoleKey.M:
                        ListMacs();
                        break;
                }
            }
        }

        private void SendDiscovery(IPEndPoint epBroad)
        {
            Console.WriteLine("Sending discovery packet...");
            var initBytes = new byte[] { (byte)ConfigCommand.Discover, 0x00, 0x00 };

            client.Send(initBytes, initBytes.Length, epBroad);
        }
        private void SendWrite(IPEndPoint epBroad)
        {
            var config = new ConfigData()
                {
                    SubnetMask = new byte[] { 255, 0, 0, 0 },
                    StatusTransmitPort = 1150,
                    ControlReceivePort = 1110
                };
            ListMacs();
            Console.Write("Select MAC address:");
            config.MacAddress = macAddresses[int.Parse(Console.ReadLine())];
            Console.Write("Enter team number:");
            config.TeamNumber = ushort.Parse(Console.ReadLine());
            Console.Write("Enter network:");
            config.Network = byte.Parse(Console.ReadLine());
            Console.Write("Enter host number:");
            config.HostIp = byte.Parse(Console.ReadLine());

            Console.WriteLine("\r\nSending configuration to " + lastDeviceId);

            var data = config.ToBytes();
            byte[] transmitData;
            using (MemoryStream stream = new MemoryStream())
            {
                var writer = new EndianBinaryWriter(EndianBitConverter.Big, stream);

                writer.Write((byte)ConfigCommand.Write);
                writer.Write(lastDeviceId);
                writer.Write((byte)0);
                writer.Write(data);

                transmitData = stream.ToArray();
            }
            client.Send(transmitData, transmitData.Length, epBroad);
        }
        private void SendErase(IPEndPoint epBroad)
        {
            byte[] transmitData;
            using (MemoryStream stream = new MemoryStream())
            {
                var writer = new EndianBinaryWriter(EndianBitConverter.Big, stream);

                writer.Write((byte)ConfigCommand.Erase);
                writer.Write(lastDeviceId);

                transmitData = stream.ToArray();
            }
            client.Send(transmitData, transmitData.Length, epBroad);
        }
        private void ListMacs()
        {
            for (int i = 0; i < macAddresses.Count; i++)
            {
                Console.WriteLine(String.Format("{0} : {1}", i, GetMacString(macAddresses[i])));
            }
        }
        private string GetMacString(byte[] mac)
        {
            var builder = new StringBuilder();
            for (int i = 0; i < mac.Length; i++)
            {
                builder.Append(mac[i].ToString("x2"));
                if (i < 5)
                {
                    builder.Append(":");
                }
            }
            return builder.ToString();
        }
        private void ProcessData(byte[] data)
        {
            var responseType = (ConfigResponse)data[2];
            switch (responseType)
            {
                case ConfigResponse.Nck:
                    Console.WriteLine("Command Failed");
                    break;
                case ConfigResponse.Ack:
                    Console.WriteLine("Command Succeeded");
                    break;
                case ConfigResponse.MacAddress:
                    var reader = new EndianBinaryReader(new BigEndianBitConverter(), new MemoryStream(data));
                    var id = reader.ReadUInt16();
                    lastDeviceId = id;
                    Console.WriteLine("Device ID: " + id);

                    var mac = reader.ReadBytes(6);
                    Console.WriteLine("MAC: " + GetMacString(mac));

                    break;
                case ConfigResponse.Data:
                    Console.WriteLine("Configuration data received");
                    break;
            }
            Console.WriteLine();
        }
        public void ReceiveData(IAsyncResult ar)
        {
            try
            {
                IPEndPoint endpoint = null;
                var data = client.EndReceive(ar, ref endpoint);
                ProcessData(data);
            }
            finally
            {
                client.BeginReceive(this.ReceiveData, null);
            }
        }
    }

    enum ConfigCommand : byte
    {
        Discover = 0x01,
        Read = 0x02,
        Write = 0x03,
        Erase = 0x04
    }

    enum ConfigResponse : byte
    {
        Nck = 0,
        Ack = 1,
        MacAddress = 2,
        Data = 3
    }

    class ConfigData
    {
        public ushort TeamNumber { get; set; }
        public byte[] MacAddress { get; set; }
        public byte Network { get; set; }
        public byte HostIp { get; set; }
        public byte[] SubnetMask { get; set; }
        public byte[] GatewayIp { get; set; }
        public ushort StatusTransmitPort { get; set; }
        public ushort ControlReceivePort { get; set; }

        public byte[] ToBytes()
        {
            byte[] data;
            using (MemoryStream stream = new MemoryStream())
            {
                var writer = new EndianBinaryWriter(EndianBitConverter.Big, stream);

                writer.Write(TeamNumber);

                if (MacAddress != null && MacAddress.Length == 6)
                    writer.Write(MacAddress);
                else
                    writer.Write(new byte[6]);

                writer.Write(Network);
                writer.Write(HostIp);

                if (SubnetMask != null && SubnetMask.Length == 4)
                    writer.Write(SubnetMask);
                else
                    writer.Write(new byte[4]);

                if (GatewayIp != null && GatewayIp.Length == 4)
                    writer.Write(GatewayIp);
                else
                    writer.Write(new byte[4]);

                writer.Write(StatusTransmitPort);
                writer.Write(ControlReceivePort);

                data = stream.ToArray();
            }

            return data;
        }

        public void FromBytes(byte[] data)
        {

        }
    }
}

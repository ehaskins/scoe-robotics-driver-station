using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Diagnostics;

namespace NetworkTestCli
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
            {
                socket.Bind(new IPEndPoint(IPAddress.Any, 1110));
                var ep = new IPEndPoint(IPAddress.Parse("172.16.92.198"), 1110);
                var buffer = new byte[1024];
                while (true)
                {
                    var count = int.Parse(Console.ReadLine());
                    var data = new byte[count];
                    socket.SendTo(data, ep);
                }
            }
        }
    }
}

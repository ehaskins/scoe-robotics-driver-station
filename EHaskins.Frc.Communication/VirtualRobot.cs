using System;
using System.Net.Sockets;
using System.Net;
using MicroLibrary;
using EHaskins.Frc.Dashboard;
using System.Diagnostics;

namespace EHaskins.Frc.Communication
{
    public class VirtualRobot
    {
        bool _isOpen;

        StatusData _status;
        UdpClient _transmitClient;
        UdpClient _receviceClient;

        bool _isConnected = false;
        private int _invalidPacketCount = 0;
        private int _packetCount = 0;

        private ushort _teamNumber = 1103;
        public VirtualRobot(ushort teamNumber)
        {
            this.TeamNumber = teamNumber;

            TransmitPort = Configuration.RobotToDsDestinationPortNumber;
            ReceivePort = Configuration.DsToRobotDestinationPortNumber;
            UserControlDataLength = Configuration.UserControlDataSize;
            UserStatusDataLength = Configuration.UserStatusDataSize;
        }

        public event EventHandler NewDataReceived;
        public int TransmitPort { get; set; }
        public int ReceivePort { get; set; }
        public int UserStatusDataLength { get; set; }
        public int UserControlDataLength { get; set; }
        public bool IsConnected
        {
            get { return _isConnected; }
            set { _isConnected = value; }
        }
        public CommandData CommandData { get; set; }
        public StatusData StatusData
        {
            get { return _status; }
        }
        public int InvalidPacketCount
        {
            get { return _invalidPacketCount; }
            set { _invalidPacketCount = value; }
        }
        public ushort TeamNumber
        {
            get { return _teamNumber; }
            set { _teamNumber = value; }
        }


        public void Start()
        {
            //_status = new StatusData(UserStatusDataLength); //TODO:FIX THIS!

            _status.TeamNumber = this.TeamNumber;

            _transmitClient = new UdpClient();

            _receviceClient = new UdpClient(ReceivePort);
            _isOpen = true;
            _receviceClient.BeginReceive(this.ReceiveData, null);
        }

        private void SendReply(CommandData packet, EndPoint endpoint)
        {
            if (_status.ReplyId > packet.PacketId & !packet.Mode.Resync)
            {
                IsConnected = false;
                return;
            }
            _status.ReplyId = packet.PacketId;
            //_status.ControlData = new ControlData(); // packet.Mode.Clone();//TODO:FIX

            var sendData = new byte[1024]; //_status.GetBytes(); //TODO:FIX
            IPEndPoint ipep = (IPEndPoint)endpoint;

            ipep.Port = TransmitPort;

            _transmitClient.Send(sendData, sendData.Length, ipep);
        }

        public void ReceiveData(IAsyncResult ar)
        {
            try
            {
                IPEndPoint endpoint = null;
                var bytes = _receviceClient.EndReceive(ar, ref endpoint);
                var packet = ParseBytes(bytes);
                if (packet != null && packet.IsValid && packet.TeamNumber == this.TeamNumber)
                {
                    SendReply(packet, endpoint);
                    if (CommandData != null && CommandData.Mode.EStop)
                    {
                        packet.Mode.EStop = true;
                    }
                    CommandData = packet;
                    if (NewDataReceived != null)
                    {
                        NewDataReceived(this, null);
                    }
                }
                else
                {
                    InvalidPacketCount += 1;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                if (_isOpen)
                {
                    _receviceClient.BeginReceive(this.ReceiveData, null);
                }
            }
        }

        private CommandData ParseBytes(byte[] data)
        {
            try
            {
                _packetCount += 1;
                return new CommandData(data, UserControlDataLength);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }
    }
}

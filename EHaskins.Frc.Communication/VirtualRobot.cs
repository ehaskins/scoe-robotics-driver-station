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
        private int _missedPacketCount = 0;
        private int _packetCount = 0;

        private int _teamNumber = 1103;
        public VirtualRobot(int teamNumber)
        {
            Start(teamNumber);
        }

        public event EventHandler NewDataReceived;

        private void Start(int teamNumber)
        {
            _status = new StatusData();

            this.TeamNumber = teamNumber;

            _status.TeamNumber = this.TeamNumber;

            _transmitClient = new UdpClient();

            _receviceClient = new UdpClient(Configuration.DsToRobotLocalPortNumber);
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
            _status.ControlData = packet.Mode.Clone();

            var sendData = _status.CreateStatusPacket();
            IPEndPoint ipep = (IPEndPoint)endpoint;

            ipep.Port = Configuration.DsToRobotLocalPortNumber;

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
                    Debug.WriteLine(packet.PacketId);
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
                return new CommandData(data);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public bool IsConnected
        {
            get { return _isConnected; }
            set { _isConnected = value; }
        }
        private CommandData _commandData;
        public CommandData CommandData
        {
            get { return _commandData; }
            set { _commandData = value; }
        }
        public StatusData StatusData
        {
            get { return _status; }
        }

        public int InvalidPacketCount
        {
            get { return _invalidPacketCount; }
            set { _invalidPacketCount = value; }
        }

        public int TeamNumber
        {
            get { return _teamNumber; }
            set { _teamNumber = value; }
        }

    }
}

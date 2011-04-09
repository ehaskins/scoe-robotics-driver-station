using System;
using Microsoft.SPOT;
namespace EHaskins.Frc.Communication
{
    public class Robot
    {
        bool _isOpen;

        StatusData _status;
        Transceiver _connection;

        private int _packetCount = 0;

        public Robot(ushort teamNumber)
        {
            this.TeamNumber = teamNumber;

            TransmitPort = 1150;
            ReceivePort = 1110;
            UserControlDataLength = 104;
            UserStatusDataLength = 152;
        }

        public event EventHandler NewDataReceived;
        public Transceiver Connection
        {
        		get
        		{
        				return _connection;
        		}
        		set
        		{
        				_connection = value;
        		}
        }
        public int TransmitPort { get; set; }
        public int ReceivePort { get; set; }
        public int UserStatusDataLength { get; set; }
        public int UserControlDataLength { get; set; }
        public bool IsConnected { get; set; }
        public ControlData ControlData { get; set; }
        public StatusData StatusData
        {
            get { return _status; }
        }
        public int InvalidPacketCount { get; set; }
        public ushort TeamNumber { get; set; }


        public void Start()
        {
            Debug.Print("Starting team " + TeamNumber + "on receive:" + ReceivePort + ", transmit:" + TransmitPort);
            _status = new StatusData();
            _status.UserStatusDataLength = UserControlDataLength;

            _status.TeamNumber = this.TeamNumber;

            ControlData = new ControlData();

            Connection = new UdpTransmitter() { ReceivePort = ReceivePort, TransmitPort = TransmitPort };
            Connection.DataReceived += new DataReceivedEventHandler(DataReceived);
            Connection.Start();
            _isOpen = true;
        }

        void DataReceived(object sender, DataReceivedEventArgs e)
        {
            try
            {
                bool lastEStop = ControlData.Mode.IsEStop;
                var bytes = e.Data;
                if (bytes.IsValidFrcPacket())
                    ParseBytes(bytes);

                if (ControlData != null && ControlData.TeamNumber == this.TeamNumber)
                {
                    SendReply(ControlData);

                    if (ControlData != null && lastEStop)
                    {
                        ControlData.Mode.IsEStop = true;
                    }
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
                Debug.Print("Error parsing data:" + ex.Message);
            }
        }

        private void SendReply(ControlData packet)
        {
            if (_status.ReplyId > packet.PacketId & !packet.Mode.IsResync)
            {
                IsConnected = false;
                return;
            }
            _status.ReplyId = packet.PacketId;
            //_status.ControlData = new ControlData(); // packet.Mode.Clone();//TODO:FIX

            Connection.Transmit(_status.GetBytes());
        }

        private void ParseBytes(byte[] data)
        {
            try
            {
                _packetCount += 1;
                ControlData.Update(data);
            }
            catch (Exception ex)
            {
                Debug.Print("Error updating CommandData: " + ex.Message);
                throw;
            }
        }
    }
}

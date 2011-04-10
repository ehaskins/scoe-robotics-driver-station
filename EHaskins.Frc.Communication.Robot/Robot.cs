using System;
using Microsoft.SPOT;
using EHaskins.Utilities;
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
                    value.TeamNumber = TeamNumber;
        			_connection = value;
        		}
        }
        public int UserStatusDataLength { get; set; }
        public int UserControlDataLength { get; set; }
        public bool IsConnected { get; set; }
        public ControlData ControlData { get; set; }
        public StatusData StatusData
        {
            get { return _status; }
        }
        public int InvalidPacketCount { get; set; }
        private ushort _TeamNumber;
        public ushort TeamNumber
        {
            get
            {
                return _TeamNumber;
            }
            set
            {
                _TeamNumber = value;
                if (Connection != null)
                    Connection.TeamNumber = value;
            }
        }


        public void Start()
        {
            Debug.Print("Starting team " + TeamNumber);
            _status = new StatusData();
            _status.UserStatusDataLength = UserControlDataLength;

            _status.TeamNumber = this.TeamNumber;

            ControlData = new ControlData();
            Connection.DataReceived += new DataReceivedEventHandler(DataReceived);
            Connection.Start();
            _isOpen = true;
        }

        void DataReceived(object sender, DataReceivedEventArgs e)
        {
            var sw = new CrappyStopwatch();
            try
            {
                bool lastEStop = ControlData.Mode.IsEStop;
                var bytes = e.Data;
                sw.PrintElapsed("starting");
                if (bytes.IsValidFrcPacket())
                {
                    sw.PrintElapsed("Verified");
                    ParseBytes(bytes);
                    sw.PrintElapsed("Parsed");
                }
                if (ControlData != null && ControlData.TeamNumber == this.TeamNumber)
                {
                    SendReply(ControlData);
                    sw.PrintElapsed("ReplySent");
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
                sw.PrintElapsed("Done");
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

            Connection.Transmit(_status.GetBytes(Connection.PacketSize));
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

using System;
using EHaskins.Utilities;
#if NETMF
using Microsoft.SPOT;
#else
using System.Diagnostics;
using System.Timers;
#endif
namespace EHaskins.Frc.Communication.Robot
{
    public class Robot
    {
        bool _IsEnabled;

        StatusData _status;
        Transceiver _connection;
#if !NETMF        
        Timer _watchDogTimer;
#endif
        private int _packetCount = 0;

        DateTime _lastPacketTime;
        int maxInterval = 500;

        public event EventHandler Connected;

        protected void RaiseConnected()
        {
            if (Connected != null)
                Connected(this, null);
        }

        public event EventHandler Disconnected;
        public void RaiseDisconnected()
        {
            if (Disconnected != null)
                Disconnected(this, null);
        }
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
        public bool IsEnabled
        {
            get
            {
                return _IsEnabled;
            }
            set
            {
                if (value && !IsEnabled)
                {
                    Start();
                }
                else if (!value && IsEnabled)
                {
                    Stop();
                }
            }
        }
        public int UserStatusDataLength { get; set; }
        public int UserControlDataLength { get; set; }
        private bool _IsConnected;
        public bool IsConnected
        {
            get
            {
                return _IsConnected;
            }
            set
            {
                if (_IsConnected == value)
                    return;
                _IsConnected = value;
                if (value)
                    RaiseConnected();
                else
                    RaiseDisconnected();
            }
        }
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


#if !NETMF
        private void WatchDogElapsed(object sender, EventArgs e)
        {
            var now = DateTime.Now;
            var max = TimeSpan.FromMilliseconds(maxInterval);
            if (now - _lastPacketTime > max)
            {
                IsConnected = false;
            }
        }
#endif
        public void Start()
        {
            Debug.Print("Starting team " + TeamNumber);
#if !NETMF
            _watchDogTimer = new Timer(250);
            _watchDogTimer.Elapsed += WatchDogElapsed;
            _watchDogTimer.Enabled = true;
#endif
            _status = new StatusData();
            _status.UserStatusDataLength = UserControlDataLength;

            _status.TeamNumber = this.TeamNumber;

            ControlData = new ControlData();
            Connection.DataReceived += new DataReceivedEventHandler(DataReceived);
            Connection.Start();
            _IsEnabled = true;
        }
        public void Stop()
        {
            if (!IsEnabled)
                throw new InvalidOperationException("DriverStation is already closed.");
            if (IsEnabled)
            {
                _IsEnabled = false;
            }
            if (Connection != null)
            {
                Connection.Stop();
            }
        }
        void DataReceived(object sender, DataReceivedEventArgs e)
        {
            try
            {
                bool lastEStop = ControlData.Mode.IsEStop;
                var bytes = e.Data;
                if (bytes.IsValidFrcPacket())
                {
                    ParseBytes(bytes);
                    _lastPacketTime = DateTime.Now;

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
            }
            catch (Exception ex)
            {
                Debug.Print("Error parsing data:" + ex.Message);
            }
        }

        private void SendReply(ControlData packet)
        {
            int elapsedPackets = _status.ReplyId < packet.PacketId ? _status.ReplyId - packet.PacketId - 1 : ushort.MaxValue - _status.ReplyId + packet.PacketId;
            if (elapsedPackets < 25)
                IsConnected = true;
            else
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

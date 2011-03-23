using System;
using System.Net.Sockets;
using System.Net;
using MicroLibrary;
using System.Diagnostics;
using System.ComponentModel;

namespace EHaskins.Frc.Communication.DriverStation
{
    public class DriverStation : IDisposable, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void RaisePropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }
        public event EventHandler NewDataReceived;
        public event EventHandler SendingData;

        bool _isOpen;

        MicroTimer _transmitTimer;
        UdpClient _client;
        IPEndPoint _transmitEP;

        public DriverStation(ushort teamNumber)
        {
            ControlData = new ControlData { TeamNumber = teamNumber, UserControlDataLength = Configuration.UserControlDataSize };
            ReceivePort = Configuration.RobotToDsDestinationPortNumber;
            TransmitPort = Configuration.DsToRobotDestinationPortNumber;


        }

        protected void InvalidateConnection()
        {
            //TODO: Finish this.
        }

        
        private byte _Network;
        public byte Network
        {
            get { return _Network; }
            set
            {
                if (_Network == value)
                    return;
                _Network = value;
                RaisePropertyChanged("Network");
                InvalidateConnection();
            }
        }
        private byte _HostNumber;
        public byte HostNumber
        {
            get { return _HostNumber; }
            set
            {
                if (_HostNumber == value)
                    return;
                _HostNumber = value;
                RaisePropertyChanged("HostNumber");
                    InvalidateConnection();
            }
        }
              
        

        private int _TransmitPort;
        public int TransmitPort
        {
            get
            {
                return _TransmitPort;
            }
            set
            {
                if (_TransmitPort == value)
                    return;
                _TransmitPort = value;
                RaisePropertyChanged("TransmitPort");
                InvalidateConnection();
            }
        }
        private int _ReceivePort;
        public int ReceivePort
        {
            get
            {
                return _ReceivePort;
            }
            set
            {
                if (_ReceivePort == value)
                    return;
                _ReceivePort = value;
                RaisePropertyChanged("ReceivePort");
                InvalidateConnection();
            }
        }
        private bool _IsSyncronized;
        public bool IsSyncronized
        {
            get
            {
                return _IsSyncronized;
            }
            protected set
            {
                if (_IsSyncronized == value)
                    return;
                _IsSyncronized = value;
                RaisePropertyChanged("IsSyncronized");
            }
        }
        private ControlData _ControlData;
        public ControlData ControlData
        {
            get
            {
                return _ControlData;
            }
            protected set
            {
                _ControlData = value;
                RaisePropertyChanged("ControlData");
            }
        }
        private StatusData _StatusData;
        public StatusData StatusData
        {
            get
            {
                return _StatusData;
            }
            protected set
            {
                _StatusData = value;
                RaisePropertyChanged("StatusData");
            }
        }
        private int _TotalInvalidPacketCount;
        public int TotalInvalidPacketCount
        {
            get
            {
                return _TotalInvalidPacketCount;
            }
            set
            {
                if (_TotalInvalidPacketCount == value)
                    return;
                _TotalInvalidPacketCount = value;
                RaisePropertyChanged("TotalInvalidPacketCount");
            }
        }
        private int _CurrentInvalidPacketCount;
        public int CurrentInvalidPacketCount
        {
            get
            {
                return _CurrentInvalidPacketCount;
            }
            set
            {
                if (_CurrentInvalidPacketCount == value)
                    return;
                _CurrentInvalidPacketCount = value;
                RaisePropertyChanged("CurrentInvalidPacketCount");
            }
        }
        private bool _SafteyTriggered;
        private int _CurrentMissedPackets;
        public int CurrentMissedPackets
        {
            get
            {
                return _CurrentMissedPackets;
            }
            set
            {
                if (_CurrentMissedPackets == value)
                    return;
                _CurrentMissedPackets = value;
                RaisePropertyChanged("CurrentMissedPackets");
            }
        }
        public bool SafteyTriggered
        {
            get
            {
                return _SafteyTriggered;
            }
            set
            {
                if (_SafteyTriggered == value)
                    return;
                _SafteyTriggered = value;
                RaisePropertyChanged("SafteyTriggered");
            }
        }



        public void Open(ushort teamNumber, IPEndPoint transmitEP = null)
        {
            ControlData.TeamNumber = teamNumber;
            _transmitEP = transmitEP ?? new IPEndPoint(FrcPacketUtils.GetIP(teamNumber, Devices.Robot), TransmitPort);

            _client = new UdpClient(ReceivePort);

            _isOpen = true;
            _client.BeginReceive(this.ReceiveData, null);

            _transmitTimer = new MicroTimer(20 * 1000);
            _transmitTimer.Elapsed += this.SendData;
            //_transmitTimer.AutoReset = True
            _transmitTimer.Start();
        }

        public void Close()
        {
            if (_transmitTimer.Enabled)
                _transmitTimer.Stop();
            if (_isOpen)
                _isOpen = false;
        }

        private void CheckSafties()
        {           
            int currentPacket = (int)ControlData.PacketId;
            if (StatusData != null)
            {
                int lastPacket = (int)StatusData.ReplyId;
                if (currentPacket > lastPacket)
                {
                    CurrentMissedPackets = currentPacket - lastPacket;
                }
                else
                {
                    CurrentMissedPackets = ushort.MaxValue - lastPacket + currentPacket;
                }
            }
            else
            {
                CurrentMissedPackets = currentPacket;
            }
            if (CurrentInvalidPacketCount > Configuration.InvalidPacketCountSafety ||
                CurrentMissedPackets > Configuration.InvalidPacketCountSafety)
            {
                //TODO: Raise event here.
                SafteyTriggered = true;
            }
            else
            {
                SafteyTriggered = false;
            }
        }

        private void UpdateMode()
        {
            if (StatusData != null && StatusData.ReplyId == ControlData.PacketId && StatusData.Mode.IsEStop)
                ControlData.Mode.IsEStop = true;

            if (SafteyTriggered)
            {
                ControlData.Mode.IsEnabled = false;
                ControlData.Mode.IsEStop = false;
                IsSyncronized = false;
            }

            if (ControlData.PacketId == UInt16.MaxValue)
            {
                ControlData.PacketId = 0;
                IsSyncronized = false;
            }

            if (!IsSyncronized)
            {
                ControlData.Mode.IsResync = true;
            }
            else
            {
                ControlData.Mode.IsResync = false;
            }
            ControlData.PacketId += 1;
        }

        public void SendData(object sender, MicroTimerEventArgs e)
        {
            if (!_isOpen)
            {
                if (_client != null)
                    _client.Close();
                return;
            }

            UpdateMode();
            CheckSafties();
            if (SendingData != null)
            {
                SendingData(this, null);
            }
            var sendData = ControlData.GetBytes();
            _client.Send(sendData, sendData.Length, _transmitEP);
        }

        public void ReceiveData(IAsyncResult ar)
        {
            try
            {
                IPEndPoint endpoint = null;
                var bytes = _client.EndReceive(ar, ref endpoint);
                ParseBytes(bytes);
            }
            catch (Exception ex)
            {
                //TODO: Log me!
            }
            finally
            {
                if (_isOpen)
                {
                    _client.BeginReceive(this.ReceiveData, null);
                }
            }
        }

        private bool ReceiveCheck(StatusData status)
        {
            return status.TeamNumber == ControlData.TeamNumber && status.ReplyId == ControlData.PacketId;
        }
        private void ParseBytes(byte[] data)
        {
            try
            {
                if (data.IsValidFrcPacket())
                {
                    var status = new StatusData(); // new StatusData(data, UserStatusDataLength); //TODO:FIX
                    status.Update(data);
                    if (ReceiveCheck(status))
                    {
                        CurrentInvalidPacketCount = 0;
                        IsSyncronized = true;
                        SafteyTriggered = false;
                        StatusData = status;

                        if (NewDataReceived != null)
                        {
                            NewDataReceived(this, null);
                        }
                    }
                    else
                    {
                        CurrentInvalidPacketCount++;
                    }
                }
                else
                {
                    CurrentInvalidPacketCount++;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }

        }

        #region "IDisposable Support"
        // To detect redundant calls
        private bool disposedValue;

        // IDisposable
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
                if (disposing)
                {
                    this.Close();
                }

            }
            this.disposedValue = true;
        }

        // This code added by Visual Basic to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

    }
}


using System;
using System.Net.Sockets;
using System.Net;
using MicroLibrary;
using System.Diagnostics;
using System.ComponentModel;
using System.Threading;
using System.Windows.Threading;

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

        public event EventHandler Starting;
        public void RaiseStarting()
        {
            if (Starting != null)
                Starting(this, null);
        }

        public event EventHandler Started;
        public void RaiseStarted()
        {
            if (Started != null)
                Started(this, null);
        }
        
        public event EventHandler NewDataReceived;
        public event EventHandler SendingData;

        bool _isEnabled;
        bool _isStopped;

        MicroTimer _transmitTimer;
        UdpClient _client;
        IPEndPoint _transmitEP;

        public DriverStation()
        {
            Network = 10;
            HostNumber = 2;
            UserControlDataSize = Configuration.UserControlDataSize;
            ReceivePort = Configuration.RobotToDsDestinationPortNumber;
            TransmitPort = Configuration.DsToRobotDestinationPortNumber;
        }

        protected void InvalidateConnection()
        {
            try
            {
                if (IsEnabled)
                {
                    Stop(false);
                    //SpinWait.SpinUntil(() => _isStopped == true);
                    Start();
                }
            }
            catch
            {
                throw;
            }
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

                InvalidateConnection();
                RaisePropertyChanged("Network");
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

                InvalidateConnection();
                RaisePropertyChanged("HostNumber");
            }
        }
        private ushort _TeamNumber;
        public ushort TeamNumber
        {
            get
            {
                return _TeamNumber;
            }
            set
            {
                if (_TeamNumber == value)
                    return;
                _TeamNumber = value;
                InvalidateConnection();
                RaisePropertyChanged("TeamNumber");
            }
        }
        private int _UserControlDataSize;
        public int UserControlDataSize
        {
            get { return _UserControlDataSize; }
            set
            {
                if (_UserControlDataSize == value)
                    return;
                _UserControlDataSize = value;
                InvalidateConnection();
                RaisePropertyChanged("UserControlDataSize");
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

                InvalidateConnection();
                RaisePropertyChanged("TransmitPort");
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

                InvalidateConnection();
                RaisePropertyChanged("ReceivePort");
            }
        }
        public bool IsEnabled
        {
            get
            {
                return _isEnabled;
            }
            set
            {
                if (value && !IsEnabled)
                {
                    Start();
                    RaisePropertyChanged("IsEnabled");
                }
                else if (!value && IsEnabled)
                {
                    Stop();
                    RaisePropertyChanged("IsEnabled");
                }
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
                if (_ControlData == value)
                    return;
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



        public void Start()
        {
            RaiseStarting();
            ControlData = new ControlData(TeamNumber) { UserControlDataLength = Configuration.UserControlDataSize };

            _transmitEP = new IPEndPoint(FrcPacketUtils.GetIP(Network, TeamNumber, HostNumber), TransmitPort);

            _client = new UdpClient(ReceivePort);

            _isEnabled = true;
            _client.BeginReceive(this.ReceiveData, null);

            _transmitTimer = new MicroTimer(20 * 1000);
            _transmitTimer.Elapsed += this.SendData;
            //_transmitTimer.AutoReset = True
            RaiseStarted();
            _transmitTimer.Start();
        }

        public void Stop()
        {
            Stop(false);
        }
        protected void Stop(bool reset)
        {
            //TOOD: Add stopped/stoping events
            //ControlData.Dispose();

            if (_transmitTimer.Enabled)
                _transmitTimer.Stop();
            if (_isEnabled)
                _isEnabled = false;

            if(!reset)
                ControlData = null;

            if (_client != null)
            {
                _client.Close();
            }
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
            if (!IsEnabled)
            {
                return;
            }
            //HACK: Why do I need this, and why does it fix the binding issue.
            if (ControlData.PacketId == 0)
                RaisePropertyChanged("ControlData");
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
                if (_client != null && IsEnabled)
                {
                    IPEndPoint endpoint = null;
                    var bytes = _client.EndReceive(ar, ref endpoint);
                    ParseBytes(bytes);
                }
            }
            catch (Exception ex)
            {
                //TODO: Log me!
            }
            finally
            {
                if (IsEnabled)
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
                    this.Stop();
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


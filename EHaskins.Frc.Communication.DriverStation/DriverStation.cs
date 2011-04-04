using System;
using System.Net.Sockets;
using System.Net;
using MicroLibrary;
using System.Diagnostics;
using System.ComponentModel;
using System.Threading;
using System.Windows.Threading;
using System.Collections.Generic;

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

        MicroTimer _transmitTimer;

        public DriverStation()
        {
            UserControlDataSize = Configuration.UserControlDataSize;
        }

        protected void InvalidateConnection()
        {
            try
            {
                if (IsEnabled)
                {
                    Stop(false);
                    Start();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        private Transceiver _Connection;
        public Transceiver Connection
        {
            get
            {
                return _Connection;
            }
            set
            {
                if (_Connection == value)
                    return;
                if (_Connection != null)
                    _Connection.DataReceived -= this.DataReceived;
                _Connection = value;
                _Connection.TeamNumber = TeamNumber;
                _Connection.DataReceived += this.DataReceived;
                _Connection.ConnectionReset += this.ConnectionReset;
                RaisePropertyChanged("Connection");
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
                if (Connection != null)
                    Connection.TeamNumber = value;
                RaisePropertyChanged("TeamNumber");
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
                /*if (value != null)
                    value.Joysticks = Joysticks.ToArray();*/
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
        private List<Joystick> _Joysticks;
        public List<Joystick> Joysticks
        {
            get
            {
                return _Joysticks;
            }
            set
            {
                if (_Joysticks == value)
                    return;
                _Joysticks = value;
                RaisePropertyChanged("Joysticks");
            }
        }

        private ControlData GetNewControlData()
        {
            return new ControlData(TeamNumber) { UserControlDataLength = UserControlDataSize };
        }
        public void Start()
        {
            try
            {
                RaiseStarting();
                ControlData = GetNewControlData();

                _isEnabled = true;

                if (!Connection.IsEnabled)
                    Connection.Start();

                _transmitTimer = new MicroTimer(20 * 1000);
                _transmitTimer.Elapsed += this.SendData;
                //_transmitTimer.AutoReset = True
                RaiseStarted();
                _transmitTimer.Start();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public void Stop()
        {
            Stop(false);
        }
        protected void Stop(bool reset)
        {
            //TOOD: Add stopped/stoping events
            //ControlData.Dispose();
            if (!IsEnabled)
                throw new InvalidOperationException("DriverStation is already closed.");
            if (IsEnabled)
            {
                _isEnabled = false;
                RaisePropertyChanged("IsEnabled");
            }
            if (_transmitTimer.Enabled)
                _transmitTimer.Stop();
            if(!reset)
                ControlData = null;

            if (Connection != null && !reset)
            {
                Connection.Stop();
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
            try
            {
                if (!IsEnabled)
                {
                    return;
                }
                //HACK: Why do I need this, and why does it fix the binding issue (Packet number).
                if (ControlData.PacketId == 0)
                   RaisePropertyChanged("ControlData");
                UpdateMode();
                CheckSafties();
                if (SendingData != null)
                {
                    SendingData(this, null);
                }
                var data = ControlData.GetBytes();
                Connection.Transmit(data);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message + " at DriverStation.SendData");
            }
        }

        private void DataReceived(object sender, DataReceivedEventArgs e)
        {
            ParseBytes(e.Data);
        }
        private void ConnectionReset(object sender, EventArgs e)
        {
            Stop(true);
            Start();
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
                    if (IsEnabled)
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


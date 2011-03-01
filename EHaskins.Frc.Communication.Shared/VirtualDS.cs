using System;
using System.Net.Sockets;
using System.Net;
using MicroLibrary;
using System.Diagnostics;

namespace EHaskins.Frc.Communication
{
    public class VirtualDS : IDisposable
    {
        public event EventHandler NewDataReceived;
        public event EventHandler SendingData;

        bool _isOpen;

        MicroTimer _transmitTimer;
        UdpClient _client;
        IPEndPoint _transmitEP;

        public VirtualDS(ushort teamNumber)
        {
            ControlData = new ControlData();
            ControlData.TeamNumber = teamNumber;
            ControlData.UserControlDataLength = Configuration.UserControlDataSize;
            ReceivePort = Configuration.RobotToDsDestinationPortNumber;
            TransmitPort = Configuration.DsToRobotDestinationPortNumber;


        }

        public int TransmitPort { get; set; }
        public int ReceivePort { get; set; }
        public bool IsSyncronized { get; protected set; }
        public ControlData ControlData { get; protected set; }
        public StatusData StatusData { get; protected set; }
        public int TotalInvalidPacketCount { get; set; }
        public int CurrentInvalidPacketCount { get; set; }
        public bool SafteyTriggered { get; set; }

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
            if (_isOpen)
                _isOpen = false;
            if (_client != null)
                _client.Close();
            if (_transmitTimer.Enabled)
                _transmitTimer.Stop();
        }

        private void CheckSafties()
        {
            if (StatusData == null || (StatusData.ReplyId < ControlData.PacketId))
            {
                CurrentInvalidPacketCount += 1;
            }
            if (CurrentInvalidPacketCount > Configuration.InvalidPacketCountSafety)
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
            ControlData.PacketId += 1;

            if (SafteyTriggered)
            {
                ControlData.Mode.Enabled = false;
                IsSyncronized = false;
            }

            if (ControlData.PacketId == UInt16.MaxValue)
            {
                ControlData.PacketId = 0;
                IsSyncronized = false;
            }

            if (!IsSyncronized)
            {
                ControlData.Mode.Resync = true;
            }
            else
            {
                ControlData.Mode.Resync = false;
            }
        }

        public void SendData(object sender, MicroTimerEventArgs e)
        {
            CheckSafties();
            UpdateMode();
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
            return status.IsValid && status.TeamNumber == ControlData.TeamNumber && status.ReplyId == ControlData.PacketId;
        }
        private void ParseBytes(byte[] data)
        {
            try
            {
                var status = new StatusData(); // new StatusData(data, UserStatusDataLength); //TODO:FIX
                status.Update(data);
                if (ReceiveCheck(status))
                {
                    CurrentInvalidPacketCount = 0;
                    IsSyncronized = true;

                    StatusData = status;

                    if (NewDataReceived != null)
                    {
                        NewDataReceived(this, null);
                    }
                }
                else
                {
                    CurrentInvalidPacketCount += 1;
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


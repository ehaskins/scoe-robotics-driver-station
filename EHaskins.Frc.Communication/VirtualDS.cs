using System;
using System.Net.Sockets;
using System.Net;
using MicroLibrary;
using EHaskins.Frc.Dashboard;
using System.Diagnostics;

namespace EHaskins.Frc.Communication
{
    public class VirtualDS : IDisposable
    {

        bool _isOpen;

        MicroTimer _transmitTimer;
        StatusData _robotStatus;
        CommandData _controlData;
        UdpClient _transmitClient;
        UdpClient _receviceClient;
        IPEndPoint _transmitEP;
        IPEndPoint _receiveEP;

        bool _isSyncronized = false;
        private int _invalidPacketCount = 0;
        private int _totalInvalidPacketCount = 0;

        public VirtualDS(int teamNumber)
        {
            _controlData = new CommandData(1);
            _controlData.Mode.RawData = 84;

            _controlData.TeamNumber = teamNumber;
        }

        public void Open(int teamNumber, IPEndPoint transmitEP = null, int receivePort = 1150)
        {
            _transmitEP = transmitEP ?? new IPEndPoint(FrcPacketUtils.GetIP(teamNumber, Devices.Robot), 1110);

            _transmitClient = new UdpClient();

            _receviceClient = new UdpClient(receivePort);

            _isOpen = true;
            _receviceClient.BeginReceive(this.ReceiveData, null);

            _transmitTimer = new MicroTimer(20000);
            _transmitTimer.Elapsed += this.SendData;
            //_transmitTimer.AutoReset = True
            _transmitTimer.Start();
        }

        public void Close()
        {
            if (_isOpen && _transmitClient != null)
            {
                _transmitTimer.Stop();
            }
        }

        private void CheckSafties()
        {
            if (_robotStatus == null || (_robotStatus.ReplyId < CommandData.PacketId))
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
            CommandData.PacketId += 1;

            if (SafteyTriggered)
            {
                CommandData.Mode.Enabled = false;
                IsSyncronized = false;
            }

            if (!IsSyncronized | CommandData.PacketId == int.MaxValue)
            {
                _controlData.Mode.Resync = true;
            }
            else
            {
                _controlData.Mode.Resync = false;
            }


        }

        public void SendData(object sender, MicroTimerEventArgs e)
        {
            CheckSafties();
            UpdateMode();

            var sendData = _controlData.GetBytes();
            _transmitClient.Send(sendData, sendData.Length, _transmitEP);
        }

        public void ReceiveData(IAsyncResult ar)
        {
            IPEndPoint endpoint = null;
            var bytes = _receviceClient.EndReceive(ar, ref endpoint);
            ParseBytes(bytes);
        }

        private void ParseBytes(byte[] data)
        {
            try
            {
                _robotStatus = new StatusData(data);

                if (_robotStatus.IsValid)
                {
                    CurrentInvalidPacketCount = 0;
                }
                else
                {
                    CurrentInvalidPacketCount += 1;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Debug.Assert(false);
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

        public bool IsSyncronized
        {
            get { return _isSyncronized; }
            set { _isSyncronized = value; }
        }

        public CommandData CommandData
        {
            get { return _controlData; }
        }

        public int TotalInvalidPacketCount
        {
            get { return _totalInvalidPacketCount; }
            set { _totalInvalidPacketCount = value; }
        }

        public int CurrentInvalidPacketCount
        {
            get { return _invalidPacketCount; }
            set { _invalidPacketCount = value; }
        }

        private bool _safteyTriggered;
        public bool SafteyTriggered
        {
            get { return _safteyTriggered; }
            set { _safteyTriggered = value; }
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
                    // TODO: dispose managed state (managed objects).
                    this.Close();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                // TODO: set large fields to null.
            }
            this.disposedValue = true;
        }

        // TODO: override Finalize() only if Dispose(ByVal disposing As Boolean) above has code to free unmanaged resources.
        //Protected Overrides Sub Finalize()
        //    ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
        //    Dispose(False)
        //    MyBase.Finalize()
        //End Sub

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


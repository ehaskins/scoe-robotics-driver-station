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

        public event EventHandler NewDataReceived;

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
            CommandData = new CommandData(1);
            CommandData.Mode.RawValue = 84;

            CommandData.TeamNumber = teamNumber;
        }

        public void Open(int teamNumber, IPEndPoint transmitEP = null, int receivePort = 1150)
        {
            CommandData.TeamNumber = teamNumber;
            _transmitEP = transmitEP ?? new IPEndPoint(FrcPacketUtils.GetIP(teamNumber, Devices.Robot), 1110);

            _transmitClient = new UdpClient();

            _receviceClient = new UdpClient(receivePort);

            _isOpen = true;
            _receviceClient.BeginReceive(this.ReceiveData, null);

            _transmitTimer = new MicroTimer(20*1000);
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
            if (RobotStatus == null || (RobotStatus.ReplyId < CommandData.PacketId))
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

            if (CommandData.PacketId == UInt16.MaxValue)
            {
                CommandData.PacketId = 0;
                IsSyncronized = false;
            }

            if (!IsSyncronized)
            {
                CommandData.Mode.Resync = true;
            }
            else
            {
                CommandData.Mode.Resync = false;
            }

            //Console.WriteLine("Packet ID:" + CommandData.PacketId + " Mode: " + CommandData.Mode.RawValue + " Team:" + CommandData.TeamNumber);

        }

        public void SendData(object sender, MicroTimerEventArgs e)
        {
            CheckSafties();
            UpdateMode();

            var sendData = CommandData.GetBytes();
            _transmitClient.Send(sendData, sendData.Length, _transmitEP);
        }

        public void ReceiveData(IAsyncResult ar)
        {
            try
            {
                IPEndPoint endpoint = null;
                var bytes = _receviceClient.EndReceive(ar, ref endpoint);
                ParseBytes(bytes);
                if (NewDataReceived != null)
                {
                    NewDataReceived(this, null);
                }
            }
            catch (Exception ex)
            {
            }
        }

        private bool ReceiveCheck(StatusData status)
        {
            return status.IsValid && status.TeamNumber == CommandData.TeamNumber && status.ReplyId == CommandData.PacketId;
        }
        private void ParseBytes(byte[] data)
        {
            try
            {
                var status = new StatusData(data);

                if (ReceiveCheck(status))
                {
                    CurrentInvalidPacketCount = 0;
                    IsSyncronized = true;
                    RobotStatus = status;
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
            protected set { _isSyncronized = value; }
        }



        public CommandData CommandData
        {
            get { return _controlData; }
            protected set { _controlData = value; }
        }
        public StatusData RobotStatus
        {
            get { return _robotStatus; }
            protected set { _robotStatus = value; }
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


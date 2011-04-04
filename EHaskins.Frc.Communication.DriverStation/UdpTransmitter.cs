using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Net.Sockets;
using System.Net;
using System.Diagnostics;
using System.Threading;

namespace EHaskins.Frc.Communication.DriverStation
{
    public class UdpTransmitter : Transceiver
    {
        UdpClient _client;
        IPEndPoint _transmitEP;
        Thread _receieveThread;
        bool _isStopped;

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
        private byte _Host;
        public byte Host
        {
            get { return _Host; }
            set
            {
                if (_Host == value)
                    return;
                _Host = value;

                InvalidateConnection();
                RaisePropertyChanged("HostNumber");
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

        public UdpTransmitter()
            : base()
        {
            ReceivePort = Configuration.RobotToDsDestinationPortNumber;
            TransmitPort = Configuration.DsToRobotDestinationPortNumber;
            Network = 10;
            Host = 2;
            TransmitPort = 1110;
            ReceivePort = 1120;
        }

        public override void Transmit(byte[] data)
        {
            if (_client == null)
                return;
            _client.Send(data, data.Length, _transmitEP);
        }

        private void ReceiveDataSync()
        {
            _isStopped = false;
            while (IsEnabled)
            {
                try
                {
                    IPEndPoint endpoint = null;
                    var data = _client.Receive(ref endpoint);

                    RaiseDataReceived(data);
                }
                catch (SocketException ex)
                {
                    if (IsEnabled)
                        Stop();
                }
            }
            _isStopped = true;
        }
        public void ReceiveData(IAsyncResult ar)
        {
            try
            {
                if (_client != null && IsEnabled)
                {
                    IPEndPoint endpoint = null;
                    var data = _client.EndReceive(ar, ref endpoint);
                    RaiseDataReceived(data);
                }
            }
            catch (SocketException sex)
            {
                //Stop();
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

        public override void Start()
        {
            _IsEnabled = true;
            _transmitEP = new IPEndPoint(FrcPacketUtils.GetIP(Network, TeamNumber, Host), TransmitPort);

            _client = new UdpClient(ReceivePort);

            //_client.BeginReceive(this.ReceiveData, null);
            _receieveThread = new Thread((ThreadStart)this.ReceiveDataSync);
            _receieveThread.Start();
        }
        public override void Stop()
        {
            _IsEnabled = false;
            if (_client != null)
                _client.Close();
            _client = null;
            SpinWait.SpinUntil(() => _isStopped, 100);
        }
        protected override void InvalidateConnection()
        {
            try
            {
                if (IsEnabled)
                {
                    Stop();
                    //SpinWait.SpinUntil(() => _isStopped == true);
                    Start();
                }
                base.InvalidateConnection();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

    }
}

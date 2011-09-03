using System;
using System;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Diagnostics;

namespace EHaskins.Frc.Communication
{
    public class UdpPhoneTransmitter : Transceiver
    {
        Socket _socket;
        bool _isStopped;

        private IPAddress _lastAddress;
        private IPEndPoint _destEP;

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
                RaisePropertyChanged("Host");
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
        private bool _IsResponderMode;
        public bool IsResponderMode
        {
            get { return _IsResponderMode; }
            set
            {
                if (_IsResponderMode == value)
                    return;
                _IsResponderMode = value;
                RaisePropertyChanged("IsResponderMode");
                InvalidateConnection();
            }
        }

        public UdpPhoneTransmitter()
            : base()
        {
            Network = 10;
            Host = 2;
            TransmitPort = 1110;
            ReceivePort = 1120;
        }

        public override void Transmit(byte[] data)
        {
            IPEndPoint ep;
            if (IsResponderMode)
            {
                if (_lastAddress == null)
                    return;
                ep = new IPEndPoint(_lastAddress, TransmitPort);
            }
            else
            {
                ep = _destEP;
            }
            if (IsEnabled && _socket != null && ep != null)
            {
                var e = new SocketAsyncEventArgs { RemoteEndPoint = ep };
                e.SetBuffer(data, 0, data.Length);
                _socket.SendToAsync(e);
            }
        }

        IPEndPoint endpoint;
        Byte[] receiveBuffer = new byte[1024];
        private void BeginReceive()
        {
            var e = new SocketAsyncEventArgs();
            e.Completed += ReceivedData;
            e.SetBuffer(receiveBuffer, 0, receiveBuffer.Length);
            _socket.ReceiveAsync(e);
        }

        private void ReceivedData(object sender, SocketAsyncEventArgs e)
        {
            var buffer = e.Buffer;

            if (IsResponderMode)
                _lastAddress = ((IPEndPoint)e.RemoteEndPoint).Address;

            RaiseDataReceived(buffer);

            if (IsEnabled)
                BeginReceive();
        }

        public override void Start()
        {
            _IsEnabled = true;
            _destEP = new IPEndPoint(FrcPacketUtils.GetIP(Network, TeamNumber, Host), TransmitPort);
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            BeginReceive();
        }
        public override void Stop()
        {
            _IsEnabled = false;
            if (_socket != null)
                _socket.Close();
            _socket = null;
        }
        protected override void InvalidateConnection()
        {
            try
            {
                if (IsEnabled)
                {
                    Stop();
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
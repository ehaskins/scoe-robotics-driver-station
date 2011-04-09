using System;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using Microsoft.SPOT;

namespace EHaskins.Frc.Communication
{
    public class UdpTransmitter : Transceiver
    {
        private IPAddress _lastAddress = null;
        Socket _socket;
        Thread _receieveThread;
        bool _isStopped;

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
            TransmitPort = 1110;
            ReceivePort = 1120;
        }

        public override void Transmit(byte[] data)
        {
            if (_socket == null && _lastAddress != null)
                return;
            _socket.SendTo(data, new IPEndPoint(_lastAddress, TransmitPort));
        }

        byte[] buffer = new byte[1024];
        EndPoint endpoint;
        private void ReceiveDataSync()
        {
            endpoint = new IPEndPoint(IPAddress.Any, ReceivePort);
            _isStopped = false;
            while (IsEnabled)
            {
                try
                {
                    var count = _socket.ReceiveFrom(buffer, ref endpoint);
                    var shortData = new byte[count];

                    for (int i = 0; i < count; i++)
                    {
                        shortData[i] = buffer[i];
                    }
                    _lastAddress = ((IPEndPoint)endpoint).Address;
                    RaiseDataReceived(shortData);
                }
                catch (SocketException ex)
                {
                    if (IsEnabled)
                        Stop();
                }
            }
            _isStopped = true;
        }

        public override void Start()
        {
            _IsEnabled = true;

            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            _socket.Bind(new IPEndPoint(IPAddress.Any, ReceivePort));

            //_client.BeginReceive(this.ReceiveData, null);
            _receieveThread = new Thread((ThreadStart)this.ReceiveDataSync);
            _receieveThread.Start();
        }
        public override void Stop()
        {
            _IsEnabled = false;
            if (_socket != null)
                _socket.Close();
            _socket = null;
            //SpinWait.SpinUntil(() => _isStopped, 100);
            //HACK: Better spin loop.
            var count = 0;
            while (!_isStopped)
            {
                Thread.Sleep(1);
                count++;
                if (count > 100)
                    throw new Exception("Stop timedout");
            }
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
                Debug.Print(ex.Message);
                throw;
            }
        }

    }
}

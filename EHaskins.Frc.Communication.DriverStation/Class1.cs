﻿using System;
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

        public UdpTransmitter()
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

        private void BeginReceive()
        {

        }

        private void ReceivedData(SocketAsyncEventArgs e)
        {
            _isStopped = false;
            while (IsEnabled)
            {
                try
                {
                    var buffer = _socket.Receive(ref endpoint);

                    if (IsResponderMode)
                        _lastAddress = ((IPEndPoint)endpoint).Address;

                    RaiseDataReceived(buffer);

                }
                catch (Exception ex)
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
            _destEP = new IPEndPoint(FrcPacketUtils.GetIP(Network, TeamNumber, Host), TransmitPort);
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            
        }
        public override void Stop()
        {
            _IsEnabled = false;
            if (_socket != null)
                _socket.Close();
            _socket = null;
#if !NETMF
            SpinWait.SpinUntil(() => _isStopped, 100);
#else
            var count = 0;
            while (!_isStopped)
            {
                Thread.Sleep(1);
                count++;
                if (count > 100)
                    throw new Exception("Stop timedout");
            }
#endif
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
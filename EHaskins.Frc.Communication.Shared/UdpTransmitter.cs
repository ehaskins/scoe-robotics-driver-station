﻿using System;
using System.Net.Sockets;
using System.Net;
using System.Threading;
#if NETMF
using Microsoft.SPOT;
#else
using System.Diagnostics;
#endif

namespace EHaskins.Frc.Communication
{
    public class UdpTransmitter : Transceiver
    {
        UdpClient _client;
        Thread _receieveThread;
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
            if (IsEnabled && _client != null && ep != null)
                _client.Send(data, data.Length, ep);
        }

        IPEndPoint endpoint;
        private void ReceiveDataSync()
        {
            endpoint = new IPEndPoint(IPAddress.Any, ReceivePort);
            _isStopped = false;
            while (IsEnabled)
            {
                try
                {
                    var buffer = _client.Receive(ref endpoint);

                    if (IsResponderMode)
                        _lastAddress = ((IPEndPoint)endpoint).Address;

                    RaiseDataReceived(buffer);
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
            _destEP = new IPEndPoint(FrcPacketUtils.GetIP(Network, TeamNumber, Host), TransmitPort);
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
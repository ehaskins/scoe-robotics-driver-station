using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Net.Sockets;
using MiscUtil.IO;
using MiscUtil.Conversion;
using System.IO;
using System.Net;
using System.Threading;

namespace EHaskins.Frc.Communication.RobotConfig
{
    public class RobotConfigurator:INotifyPropertyChanged
    {
        IPEndPoint epBroad;
        bool _IsStopped = true;
        Thread receiveThread;
        public RobotConfigurator()
        {
            ConfigPort = 1000;
            Robots = new ObservableCollection<RobotInfo>();
            Responses = new ObservableCollection<RobotResponse>();
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void RaisePropertyChanged(string prop)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        public void Open()
        {
            if (Client != null)
                throw new InvalidOperationException("Connection already open.");
            Client = new UdpClient(ConfigPort + 1);
            _IsOpen = true;
            receiveThread = new Thread(ReceiveDataSync);
            receiveThread.Start();
        }

        public void Close()
        {
            if (Client == null)
                throw new InvalidOperationException("Connection is already closed.");
            _IsOpen = false;
            Client.Close();
            SpinWait.SpinUntil(() => _IsStopped, 100);
            Client = null;
        }
        private bool _IsOpen;
        public bool IsOpen
        {
            get { return _IsOpen; }
            set
            {
                if (value && !IsOpen)
                    Open();
                else if (!value && IsOpen)
                    Close();
                else
                    return;
                RaisePropertyChanged("IsOpen");
            }
        }
        
        private void ProcessData(byte[] data)
        {
            var response = new RobotResponse(data);
            Responses.Insert(0, response);
            switch (response.ResponseCode)
            {
                case ResponseCode.MacAddress:
                    Robots.Add(new RobotInfo(response));
                    break;
                case ResponseCode.Data:
                    //TODO:Parse read data.
                    break;
                default:
                    break;
            }
        }
        private void ReceiveDataSync()
        {
            _IsStopped = false;
            while (IsOpen)
            {
                try
                {
                    var ep = new IPEndPoint(IPAddress.Any, ConfigPort + 1);
                    var buffer = Client.Receive(ref ep);

                    ProcessData(buffer);
                }
                catch (SocketException ex)
                {
                    if (IsOpen)
                        Close();
                }
            }
            _IsStopped = true;
        }

        private ObservableCollection<RobotResponse> _Responses;
        public ObservableCollection<RobotResponse> Responses
        {
            get { return _Responses; }
            set
            {
                if (_Responses == value)
                    return;
                _Responses = value;
                RaisePropertyChanged("Responses");
            }
        }

        private ObservableCollection<RobotInfo> _Robots;
        public ObservableCollection<RobotInfo> Robots
        {
            get { return _Robots; }
            set
            {
                if (_Robots == value)
                    return;
                _Robots = value;
                RaisePropertyChanged("Robots");
            }
        }

        private UdpClient _Client;
        public UdpClient Client
        {
            get { return _Client; }
            set
            {
                if (_Client == value)
                    return;
                _Client = value;
                RaisePropertyChanged("Client");
            }
        }

        private ushort _ConfigPort;
        public ushort ConfigPort
        {
            get { return _ConfigPort; }
            set
            {
                if (_ConfigPort == value)
                    return;
                _ConfigPort = value;
                epBroad = new IPEndPoint(IPAddress.Broadcast, ConfigPort);
                RaisePropertyChanged("ConfigPort");
            }
        }
        
        public void BeginDiscovery()
        {
            var initBytes = new byte[] { (byte)ConfigCommand.Discover, 0x00, 0x00 };

            Robots.Clear();
            Client.Send(initBytes, initBytes.Length, epBroad);
        }

        public void BeginWrite(ushort deviceId, RobotConfiguration config){
             var data = config.ToBytes();
            byte[] transmitData;
            using (MemoryStream stream = new MemoryStream())
            {
                var writer = new EndianBinaryWriter(EndianBitConverter.Big, stream);

                writer.Write((byte)ConfigCommand.Write);
                writer.Write(deviceId);
                writer.Write((byte)0);
                writer.Write(data);

                transmitData = stream.ToArray();
            }
            Client.Send(transmitData, transmitData.Length, epBroad);
        }
        public void BeginErase(ushort deviceId)
        {
            byte[] transmitData;
            using (MemoryStream stream = new MemoryStream())
            {
                var writer = new EndianBinaryWriter(EndianBitConverter.Big, stream);

                writer.Write((byte)ConfigCommand.Erase);
                writer.Write(deviceId);

                transmitData = stream.ToArray();
            }
            Client.Send(transmitData, transmitData.Length, epBroad);
        }

    }
}

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

namespace EHaskins.Frc.Communication.RobotConfig
{
    class RobotConfigurator:INotifyPropertyChanged
    {
        IPEndPoint epBroad;

        public RobotConfigurator()
        {
            ConfigPort = 1000;
            Client = new UdpClient(ConfigPort + 1);
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void RaisePropertyChanged(string prop)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
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
        public void ReceiveData(IAsyncResult ar)
        {
            try
            {
                IPEndPoint endpoint = null;
                var data = Client.EndReceive(ar, ref endpoint);
                ProcessData(data);
            }
            finally
            {
                Client.BeginReceive(this.ReceiveData, null);
            }
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
        
        private ushort _ConfigPort = 1000;
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

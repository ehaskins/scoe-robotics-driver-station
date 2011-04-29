using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.IO;
using MiscUtil.IO;
using MiscUtil.Conversion;
namespace EHaskins.Frc.Communication.RobotConfig
{
    public class RobotConfiguration : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void RaisePropertyChanged(string prop)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        private byte[] _Mac;
        public byte[] Mac
        {
            get { return _Mac; }
            set
            {
                if (_Mac == value)
                    return;
                _Mac = value;
                RaisePropertyChanged("Mac");
            }
        }

        private byte[] _SubnetMask;
        public byte[] SubnetMask
        {
            get { return _SubnetMask; }
            set
            {
                if (_SubnetMask == value)
                    return;
                _SubnetMask = value;
                RaisePropertyChanged("SubnetMask");
            }
        }

        private byte[] _GatewayIp;
        public byte[] GatewayIp
        {
            get { return _GatewayIp; }
            set
            {
                if (_GatewayIp == value)
                    return;
                _GatewayIp = value;
                RaisePropertyChanged("GatewayIp");
            }
        }
        
        private ushort _Team;
        public ushort Team
        {
            get { return _Team; }
            set
            {
                if (_Team == value)
                    return;
                _Team = value;
                RaisePropertyChanged("Team");
            }
        }

        private byte _Network;
        public byte Network
        {
            get { return _Network; }
            set
            {
                if (_Network == value)
                    return;
                _Network = value;
                RaisePropertyChanged("Network");
            }
        }

        private byte _HostNumber;
        public byte HostNumber
        {
            get { return _HostNumber; }
            set
            {
                if (_HostNumber == value)
                    return;
                _HostNumber = value;
                RaisePropertyChanged("HostNumber");
            }
        }

        private ushort _StatusTransmitPort;
        public ushort StatusTransmitPort
        {
            get { return _StatusTransmitPort; }
            set
            {
                if (_StatusTransmitPort == value)
                    return;
                _StatusTransmitPort = value;
                RaisePropertyChanged("StatusTransmitPort");
            }
        }

        private ushort _ControlReceivePort;
        public ushort ControlReceivePort
        {
            get { return _ControlReceivePort; }
            set
            {
                if (_ControlReceivePort == value)
                    return;
                _ControlReceivePort = value;
                RaisePropertyChanged("ControlReceivePort");
            }
        }

        public byte[] ToBytes()
        {
            byte[] data;
            using (MemoryStream stream = new MemoryStream())
            {
                var writer = new EndianBinaryWriter(EndianBitConverter.Big, stream);

                writer.Write(Team);

                if (Mac != null && Mac.Length == 6)
                    writer.Write(Mac);
                else
                    writer.Write(new byte[6]);

                writer.Write(Network);
                writer.Write(HostNumber);

                if (SubnetMask != null && SubnetMask.Length == 4)
                    writer.Write(SubnetMask);
                else
                    writer.Write(new byte[4]);

                if (GatewayIp != null && GatewayIp.Length == 4)
                    writer.Write(GatewayIp);
                else
                    writer.Write(new byte[4]);

                writer.Write(StatusTransmitPort);
                writer.Write(ControlReceivePort);

                data = stream.ToArray();
            }

            return data;
        }
    }
}
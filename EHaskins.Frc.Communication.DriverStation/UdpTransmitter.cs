using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace EHaskins.Frc.Communication.DriverStation
{
    public class UdpTransmitter : Transmitter, INotifyPropertyChanged
    {
        public UdpTransmitter():base()
        {
            NetworkNumber = 10;
            RobotAddress = 2;
            TransmitPort = 1110;
        }

        public override void Transmit(byte[] data, int teamNumber)
        {
            
        }

        private byte _NetworkNumber;
        public byte NetworkNumber
        {
            get
            {
                return _NetworkNumber;
            }
            set
            {
                if (_NetworkNumber == value)
                    return;
                _NetworkNumber = value;
                RaisePropertyChanged("NetworkNumber");
            }
        }

        private byte _RobotAddress;
        public byte RobotAddress
        {
            get
            {
                return _RobotAddress;
            }
            set
            {
                if (_RobotAddress == value)
                    return;
                _RobotAddress = value;
                RaisePropertyChanged("RobotAddress");
            }
        }
        private ushort _TransmitPort;
        public ushort TransmitPort
        {
            get { return _TransmitPort; }
            set
            {
                if (_TransmitPort == value)
                    return;
                _TransmitPort = value;
                RaisePropertyChanged("TransmitPort");
            }
        }
        
    }
}

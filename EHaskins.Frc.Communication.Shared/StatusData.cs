using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace EHaskins.Frc.Communication
{
    public class StatusData : INotifyPropertyChanged
    {
        double _BatteryVoltage;
        public double BatteryVoltage
        {
            get
            {
                return _BatteryVoltage;
            }
            set
            {
                if (value != _BatteryVoltage)
                {
                    _BatteryVoltage = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("BatteryVoltage"));
                }
            }
        }

        bool _CodeRunning;
        public bool CodeRunning
        {
            get
            {
                return _CodeRunning;
            }
            set
            {
                if (value != _CodeRunning)
                {
                    _CodeRunning = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("CodeRunning"));
                }
            }
        }

        Mode _Mode;
        public Mode Mode
        {
            get
            {
                return _Mode;
            }
            set
            {
                if (value != _Mode)
                {
                    _Mode = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("Mode"));
                }
            }
        }

        BindableBitField8 _DigitalOutputs;
        public BindableBitField8 DigitalOutputs
        {
            get
            {
                return _DigitalOutputs;
            }
            set
            {
                if (value != _DigitalOutputs)
                {
                    _DigitalOutputs = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("DigitalOutputs"));
                }
            }
        }

        string _FpgeVersion;
        public string FpgeVersion
        {
            get
            {
                return _FpgeVersion;
            }
            set
            {
                if (value != _FpgeVersion)
                {
                    _FpgeVersion = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("FpgeVersion"));
                }
            }
        }

        bool _IsValid;
        public bool IsValid
        {
            get
            {
                return _IsValid;
            }
            set
            {
                if (value != _IsValid)
                {
                    _IsValid = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("IsValid"));
                }
            }
        }

        ushort _ReplyId;
        public ushort ReplyId
        {
            get
            {
                return _ReplyId;
            }
            set
            {
                if (value != _ReplyId)
                {
                    _ReplyId = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("ReplyId"));
                }
            }
        }

        byte[] _RobotMac;
        public byte[] RobotMac
        {
            get
            {
                return _RobotMac;
            }
            set
            {
                if (value != _RobotMac)
                {
                    _RobotMac = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("RobotMac"));
                }
            }
        }

        ushort _TeamNumber;
        public ushort TeamNumber
        {
            get
            {
                return _TeamNumber;
            }
            set
            {
                if (value != _TeamNumber)
                {
                    _TeamNumber = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("TeamNumber"));
                }
            }
        }
        byte[] _UserStatusData;
        public byte[] UserStatusData
        {
            get
            {
                return _UserStatusData;
            }
            set
            {
                if (value != _UserStatusData)
                {
                    _UserStatusData = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("UserStatusData"));
                }
            }
        }


        public event PropertyChangedEventHandler PropertyChanged = delegate { };

    }
}

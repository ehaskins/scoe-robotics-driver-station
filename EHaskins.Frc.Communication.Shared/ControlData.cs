using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace EHaskins.Frc.Communication
{
    public class ControlData : INotifyPropertyChanged
    {
        Alliance _Alliance;
        public Alliance Alliance
        {
            get
            {
                return _Alliance;
            }
            set
            {
                if (value != _Alliance)
                {
                    _Alliance = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("Alliance"));
                }
            }
        }

        ObservableCollection<ushort> _AnalogInputs;
        public ObservableCollection<ushort> AnalogInputs
        {
            get
            {
                return _AnalogInputs;
            }
            set
            {
                if (value != _AnalogInputs)
                {
                    _AnalogInputs = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("AnalogInputs"));
                }
            }
        }

        ulong _CRiochecksum;
        public ulong CRiochecksum
        {
            get
            {
                return _CRiochecksum;
            }
            set
            {
                if (value != _CRiochecksum)
                {
                    _CRiochecksum = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("CRiochecksum"));
                }
            }
        }

        BitField8 _DigitalInputs;
        public BitField8 DigitalInputs
        {
            get
            {
                return _DigitalInputs;
            }
            set
            {
                if (value != _DigitalInputs)
                {
                    _DigitalInputs = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("DigitalInputs"));
                }
            }
        }

        uint _FpgeChecksum0;
        public uint FpgeChecksum0
        {
            get
            {
                return _FpgeChecksum0;
            }
            set
            {
                if (value != _FpgeChecksum0)
                {
                    _FpgeChecksum0 = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("FpgeChecksum0"));
                }
            }
        }

        uint _FpgeChecksum1;
        public uint FpgeChecksum1
        {
            get
            {
                return _FpgeChecksum1;
            }
            set
            {
                if (value != _FpgeChecksum1)
                {
                    _FpgeChecksum1 = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("FpgeChecksum1"));
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

        ObservableCollection<Joystick> _Joysticks;
        public ObservableCollection<Joystick> Joysticks
        {
            get
            {
                return _Joysticks;
            }
            set
            {
                if (value != _Joysticks)
                {
                    _Joysticks = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("Joysticks"));
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

        uint _PacketId;
        public uint PacketId
        {
            get
            {
                return _PacketId;
            }
            set
            {
                if (value != _PacketId)
                {
                    _PacketId = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("PacketId"));
                }
            }
        }

        byte _Position;
        public byte Position
        {
            get
            {
                return _Position;
            }
            set
            {
                if (value != _Position)
                {
                    _Position = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("Position"));
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

        byte[] _UserControlData;
        public byte[] UserControlData
        {
            get
            {
                return _UserControlData;
            }
            set
            {
                if (value != _UserControlData)
                {
                    _UserControlData = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("UserControlData"));
                }
            }
        }


        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        public string Version { get; set; }
    }
}

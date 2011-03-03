using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;

namespace EHaskins.Frc.Communication
{
    public class StatusData : INotifyPropertyChanged
    {
        public StatusData()
        {
            Mode = new Mode();
            DigitalOutputs = new BindableBitField8();

        }
        public void Update(byte[] data)
        {
            using (MiscUtil.IO.EndianBinaryReader reader = new MiscUtil.IO.EndianBinaryReader(new MiscUtil.Conversion.BigEndianBitConverter(), new MemoryStream(data)))
            {
                Mode.RawValue = reader.ReadByte();
                var batteryBytes = reader.ReadBytes(2);
                BatteryVoltage = Convert.ToDouble(batteryBytes[0].ToString("x")) + (Convert.ToDouble(batteryBytes[1].ToString("x")) / 100.0);
                DigitalOutputs.RawValue = reader.ReadByte();
                reader.ReadBytes(4);
                TeamNumber = reader.ReadUInt16();
                RobotMac = reader.ReadBytes(6);
                FpgaVersion = Encoding.ASCII.GetString(reader.ReadBytes(8));
                reader.ReadBytes(6);
                ReplyId = reader.ReadUInt16();
                int userDataLength = data.Length - (int)reader.BaseStream.Position - 8;
                UserStatusDataLength = userDataLength;
                UserStatusData = reader.ReadBytes(userDataLength);
            }
        }
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
                return BatteryVoltage == 37.37;
            }
            set
            {
                if (value && !CodeRunning)
                {
                    BatteryVoltage = 0.0;
                }
                else if (!value)
                {
                    BatteryVoltage = 37.37;
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

        string _FpgaVersion;
        public string FpgaVersion
        {
            get
            {
                return _FpgaVersion;
            }
            set
            {
                if (value != _FpgaVersion)
                {
                    _FpgaVersion = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("FpgeVersion"));
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

        private int _UserStatusDataLength;
        public int UserStatusDataLength
        {
            get
            {
                return _UserStatusDataLength;
            }
            set
            {
                if (value != _UserStatusDataLength)
                {
                    _UserStatusDataLength = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("UserStatusDataLength"));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

    }
}

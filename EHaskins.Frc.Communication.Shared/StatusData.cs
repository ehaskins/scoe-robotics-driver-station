using System;
using System.Text;
using System.IO;
using System.ComponentModel;
using EHaskins.Utilities.NumericExtensions;

namespace EHaskins.Frc.Communication
{
    public class StatusData : INotifyPropertyChanged
    {
        public StatusData()
        {
            Mode = new Mode();
            DigitalOutputs = new BindableBitField8();

        }
#if !NETMF
        public void Update(byte[] data)
        {
            using (MiscUtil.IO.EndianBinaryReader reader = new MiscUtil.IO.EndianBinaryReader(new MiscUtil.Conversion.BigEndianBitConverter(), new MemoryStream(data)))
            {
                Mode.RawValue = reader.ReadByte();
                var batteryBytes = reader.ReadBytes(2);
                BatteryVoltage = (double)batteryBytes[0] + (double)batteryBytes[1] / 100.0;
                DigitalOutputs.RawValue = reader.ReadByte();
                reader.ReadBytes(4);
                TeamNumber = reader.ReadUInt16();
                RobotMac = reader.ReadBytes(6);
                FpgaVersion = Encoding.UTF8.GetString(reader.ReadBytes(8));
                reader.ReadBytes(6);
                ReplyId = reader.ReadUInt16();
                int userDataLength = data.Length - (int)reader.BaseStream.Position - 8;
                UserStatusDataLength = userDataLength;
                UserStatusData = reader.ReadBytes(userDataLength);
            }
        }
#endif
        private byte[] GetBatteryBytes()
        {
            byte[] data;
            if (CodeRunning)
            {
                var tempV = BatteryVoltage.Limit(0, 99);

                double VInt = (int)tempV;
                double VFrac = (tempV - VInt) * 100;
                //var i = byte.Parse(VInt.ToString(), System.Globalization.NumberStyles.HexNumber);
                //var f = byte.Parse(VFrac.ToString(), System.Globalization.NumberStyles.HexNumber);
                var i = (byte)VInt;
                var f = (byte)VFrac;
                data = new byte[] { i, f };
            }
            else 
                data = new byte[] { 0x37, 0x37};

            return data;
        }

        public byte[] GetBytes()
        {
            byte[] data;
            using (MemoryStream stream = new MemoryStream())
            {
                var writer = new MiscUtil.IO.EndianBinaryWriter(new MiscUtil.Conversion.BigEndianBitConverter(), stream, System.Text.Encoding.UTF8);
                writer.Write(Mode.RawValue);
                writer.Write(GetBatteryBytes());
                writer.Write(DigitalOutputs.RawValue);
                writer.Write(new byte[4]);
                writer.Write(TeamNumber);
                writer.Write(RobotMac ?? new byte[6]);
                writer.Write(FpgaVersion != null ? System.Text.Encoding.UTF8.GetBytes(FpgaVersion) : new byte[8]);
                writer.Write(new byte[6]);
                writer.Write(ReplyId);
                int length = 0;
                if (UserStatusData == null)
                    length = 0;
                else
                {
                    length = UserStatusData.Length <= UserStatusDataLength ? UserStatusData.Length : UserStatusDataLength;
                    writer.Write(UserStatusData, 0, length);
                }

                var paddingLength = (UserStatusDataLength - length) + 8;
                byte[] padding = new byte[paddingLength];
                writer.Write(padding);

                var crcData = stream.ToArray();
                stream.Position -= 4;
                writer.Write(Crc32.Compute(crcData));

                data = stream.ToArray();
                writer.Close();
            }
            return data;
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
                    RaisePropertyChanged("BatteryVoltage");
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
                    RaisePropertyChanged("Mode");
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
                    RaisePropertyChanged("DigitalOutputs");
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
                    if (value.Length != 8)
                        throw new ArgumentOutOfRangeException("FpgaVersion must be exactly 8 characters in length.");
                    _FpgaVersion = value;
                    RaisePropertyChanged("FpgeVersion");
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
                    RaisePropertyChanged("ReplyId");
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
                    RaisePropertyChanged("RobotMac");
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
                    RaisePropertyChanged("TeamNumber");
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
                    RaisePropertyChanged("UserStatusData");
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
                    RaisePropertyChanged("UserStatusDataLength");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void RaisePropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

    }
}

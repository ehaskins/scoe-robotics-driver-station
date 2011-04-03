using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Diagnostics;

namespace EHaskins.Frc.Communication
{
    public class ControlData : INotifyPropertyChanged
    {
        public ControlData(ushort teamNumber):this()
        {
            TeamNumber = teamNumber;
        }
        public ControlData()
        {
            AnalogInputs = new ObservableCollection<ushort>();
            for (int i = 0; i < 4; i++)
            {
                AnalogInputs.Add(0);
            }
            Joysticks = new ObservableCollection<Joystick>();
            for (int i = 0; i < 4; i++)
            {
                Joysticks.Add(new Joystick());
            }
            Mode = new Mode((byte)84);
            DigitalInputs = new BindableBitField8();
        }
        public void Update(byte[] data)
        {
            using (var reader = new MiscUtil.IO.EndianBinaryReader(new MiscUtil.Conversion.BigEndianBitConverter(), new MemoryStream(data)))
            {
                PacketId = reader.ReadUInt16();
                Mode = new Mode(reader.ReadByte());
                DigitalInputs = new BindableBitField8(reader.ReadByte());
                TeamNumber = reader.ReadUInt16();
                Alliance = (Alliance)reader.ReadByte();
                Position = (byte)(reader.ReadByte() - 48);
                for (int i = 0; i <= 3; i++)
                    Joysticks[i].Parse(reader);
                for (int i = 0; i <= 3; i++)
                    AnalogInputs[i] = reader.ReadUInt16();
                CRioChecksum = reader.ReadUInt64();
                FpgaChecksum0 = reader.ReadUInt32();
                FpgaChecksum1 = reader.ReadUInt32();
                FpgaChecksum2 = reader.ReadUInt32();
                FpgaChecksum3 = reader.ReadUInt32();
                Version = Encoding.ASCII.GetString(reader.ReadBytes(8));
                int userDataLength = data.Length - (int)reader.BaseStream.Position - 8;
                UserControlDataLength = userDataLength;
                UserControlData = reader.ReadBytes(userDataLength);
            }
        }
        public byte[] GetBytes()
        {
            byte[] data = null;
            using (MemoryStream stream = new MemoryStream())
            {
                MiscUtil.IO.EndianBinaryWriter writer = new MiscUtil.IO.EndianBinaryWriter(new MiscUtil.Conversion.BigEndianBitConverter(), stream, Encoding.ASCII);
                writer.Write(Convert.ToUInt16(PacketId));
                writer.Write(Mode.RawValue);
                writer.Write(DigitalInputs.RawValue);
                writer.Write((UInt16)TeamNumber);
                writer.Write(Convert.ToByte(Alliance));
                writer.Write(Convert.ToByte(48 + Position));
                foreach (var joystick in Joysticks)
                {
                    byte[] joystickData = joystick.GetBytes();
                    writer.Write(joystickData);
                }
                foreach (UInt16 input in AnalogInputs)
                {
                    writer.Write(input);
                }
                writer.Write(CRioChecksum);
                writer.Write(FpgaChecksum0);
                writer.Write(FpgaChecksum1);
                writer.Write(FpgaChecksum2);
                writer.Write(FpgaChecksum3);
                writer.Write(Version != null ? Encoding.ASCII.GetBytes(Version) : new byte[8]);

                int length = 0;
                if (UserControlData == null)
                    length = 0;
                else
                {
                    length = UserControlData.Length <= UserControlDataLength ? UserControlData.Length : UserControlDataLength;
                    writer.Write(UserControlData, 0, length);
                }

                var paddingLength = (UserControlDataLength - length) + 8;
                byte[] padding = new byte[paddingLength];
                writer.Write(padding);

                var crcData = stream.ToArray();
                stream.Position -= 4;
                writer.Write((new Crc32()).ComputeHash(crcData));

                data = stream.ToArray();
                writer.Close();
            }

            Debug.Assert(data.IsValidFrcPacket());
            return data;
        }

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

        ulong _CRioChecksum;
        public ulong CRioChecksum
        {
            get
            {
                return _CRioChecksum;
            }
            set
            {
                if (value != _CRioChecksum)
                {
                    _CRioChecksum = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("CRiochecksum"));
                }
            }
        }

        BindableBitField8 _DigitalInputs;
        public BindableBitField8 DigitalInputs
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

        uint _FpgaChecksum0;
        public uint FpgaChecksum0
        {
            get
            {
                return _FpgaChecksum0;
            }
            set
            {
                if (value != _FpgaChecksum0)
                {
                    _FpgaChecksum0 = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("FpgeChecksum0"));
                }
            }
        }

        uint _FpgaChecksum1;
        public uint FpgaChecksum1
        {
            get
            {
                return _FpgaChecksum1;
            }
            set
            {
                if (value != _FpgaChecksum1)
                {
                    _FpgaChecksum1 = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("FpgeChecksum1"));
                }
            }
        }

        private uint _FpgaChecksum2;
        public uint FpgaChecksum2
        {
            get
            {
                return _FpgaChecksum2;
            }
            set
            {
                if (value != _FpgaChecksum2)
                {
                    _FpgaChecksum2 = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("FpgeChecksum2"));
                }
            }
        }

        private uint _FpgaChecksum3;
        public uint FpgaChecksum3
        {
            get
            {
                return _FpgaChecksum3;
            }
            set
            {
                if (value != _FpgaChecksum3)
                {
                    _FpgaChecksum3 = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("FpgeChecksum3"));
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

        ushort _PacketId;
        public ushort PacketId
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
            protected set
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

        private int _UserControlDataLength;
        public int UserControlDataLength
        {
            get
            {
                return _UserControlDataLength;
            }
            set
            {
                if (value != _UserControlDataLength)
                {
                    _UserControlDataLength = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("UserControlDataLength"));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        public string Version { get; set; }
    }
}

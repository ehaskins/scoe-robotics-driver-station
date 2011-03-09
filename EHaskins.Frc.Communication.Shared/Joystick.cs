using System;
using System.IO;
using System.ComponentModel;
using MiscUtil.IO;

namespace EHaskins.Frc.Communication
{
    public class Joystick
    {

        public Joystick()
        {
            Buttons = new BindableBitField16();
        }

        int _joystickNumber;

        double[] _analogInputs = new double[6];

        public void Parse(EndianBinaryReader reader)
        {
            for (int i = 0; i < 6; i++)
            {
                int byteRead = reader.ReadByte();
                double value = (byteRead - 128) / 128;
                Axes[i] = value;
            }

            Buttons.RawValue = reader.ReadUInt16();

            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs("AnalogInputs"));
            }
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs("X"));
            }
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs("Y"));
            }
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs("Z"));
            }
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs("Twist"));
            }
        }

        public byte[] GetBytes()
        {
            MemoryStream stream = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(stream);
            for (int i = 0; i < 6; i++)
            {
                double scaled = Axes[i] * 128;
                if (scaled > SByte.MaxValue)
                    scaled = 255;
                else if (scaled < -SByte.MinValue)
                    scaled = 0;
                writer.Write(Convert.ToSByte(scaled));
            }

            writer.Write(Buttons.RawValue);

            var data = stream.ToArray();
            writer.Close();

            return data;
        }
        public double[] Axes
        {
            get { return _analogInputs; }
            set
            {
                if (_analogInputs == value)
                    return;
                _analogInputs = value;

                //RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs("AnalogInputs"))
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("X"));
                }
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Y"));
                }
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Z"));
                }
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Twist"));
                }
            }
        }
        public int JoystickNumber
        {
            get { return _joystickNumber; }
            set
            {
                if (_joystickNumber == value)
                {
                    return;
                }
                _joystickNumber = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("JoystickNumber"));
                }
            }
        }

        private BindableBitField16 _Buttons;
        public BindableBitField16 Buttons
        {
            get { return _Buttons; }
            set { _Buttons = value; }
        }

        public double X
        {
            get { return Axes[0]; }
        }
        public double Y
        {
            get { return Axes[1]; }
        }
        public double Z
        {
            get { return Axes[2]; }
        }
        public double Twist
        {
            get { return Axes[3]; }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}

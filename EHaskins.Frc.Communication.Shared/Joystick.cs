using System;
using System.IO;
using System.ComponentModel;
using MiscUtil.IO;

namespace EHaskins.Frc.Communication
{
    public class Joystick : INotifyPropertyChanged
    {

        public Joystick()
        {
        }
        public Joystick(int number)
        {
            JoystickNumber = number;
        }

        int _joystickNumber;
        double[] _analogInputs = new double[7];

        bool[] _digitalInputs = new bool[17];
        public void Parse(EndianBinaryReader reader)
        {
            for (int i = 0; i <= 5; i++)
            {
                int byteRead = reader.ReadByte();
                double value = (byteRead - 128) / 128;
                AnalogInputs[i] = value;
            }

            //TODO: Add button handling code
            object bytes = reader.ReadBytes(2);

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
            for (int i = 0; i <= 5; i++)
            {
                double scaled = AnalogInputs[i] * 128 + 128;
                if (scaled > 255)
                    scaled = 255;
                else if (scaled < 0)
                    scaled = 0;
                writer.Write(Convert.ToByte(scaled));
            }
            //TODO: Add button handling code here.
            writer.Write(Convert.ToByte(0));
            writer.Write(Convert.ToByte(0));

            var data = stream.ToArray();
            writer.Close();

            return data;
        }
        public double[] AnalogInputs
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
        public bool[] DigitalInputs
        {
            get { return _digitalInputs; }
            set
            {
                if ((!object.ReferenceEquals(_digitalInputs, value)))
                {
                    return;
                }
                _digitalInputs = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("DigitalInputs"));
                }
            }
        }

        public double X
        {
            get { return AnalogInputs[0]; }
        }
        public double Y
        {
            get { return AnalogInputs[1]; }
        }
        public double Z
        {
            get { return AnalogInputs[2]; }
        }
        public double Twist
        {
            get { return AnalogInputs[3]; }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}

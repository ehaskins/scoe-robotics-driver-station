using System;
using System.ComponentModel;
using System.Text;

namespace EHaskins.Frc.Communication
{
    public class DSVersion : INotifyPropertyChanged
    {
        byte[] _bytes;

        public event PropertyChangedEventHandler PropertyChanged;

        public DSVersion()
        {
            VersionString = "090210a1";
        }
        protected void RaisePropertyChanged(string prop)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
            }
        }

        public DSVersion(byte[] data)
        {
            if (data.Length == 8)
            {
                VersionString = Encoding.ASCII.GetString(data);
                RaisePropertyChanged("VersionString");
            }
            else
            {
                throw new ArgumentOutOfRangeException("Data must be exactly 8 bytes in length.");
            }
        }

        private string _versionString;
        public string VersionString
        {
            get { return _versionString; }
            set
            {
                if ((_versionString == value))
                    return;
                var bytes = Encoding.ASCII.GetBytes(value);
                if (value == null || value.Length != 8 || bytes.Length != 8)
                {
                    throw new ArgumentException("VersionString must be exactly 8 characters.");
                }

                _bytes = bytes;
                _versionString = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("VersionString"));
                }

            }
        }

        public byte[] GetBytes()
        {
            return _bytes;
        }
    }
}

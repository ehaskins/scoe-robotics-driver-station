using System;
using System.ComponentModel;
namespace EHaskins.Frc.Communication
{
    public abstract class Transceiver : INotifyPropertyChanged
    {
        public Transceiver()
        {

        }

        protected void RaisePropertyChanged(string prop)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaiseDataReceived(byte[] data)
        {
            if (DataReceived != null)
                DataReceived(this, new DataReceivedEventArgs(data));
        }
        public event DataReceivedEventHandler DataReceived;

        protected void RaiseConnectionReset()
        {
            if (ConnectionReset != null)
                ConnectionReset(this, null);
        }
        public event EventHandler ConnectionReset;
        private ushort _TeamNumber;
        public ushort TeamNumber
        {
            get { return _TeamNumber; }
            set
            {
                if (_TeamNumber == value)
                    return;
                _TeamNumber = value;
                InvalidateConnection();
                RaisePropertyChanged("TeamNumber");
            }
        }
        private int _PacketSize;
        public int PacketSize
        {
            get { return _PacketSize; }
            set
            {
                _PacketSize = value;
                RaisePropertyChanged("PacketSize");
                InvalidateConnection();
            }
        }
        protected bool _IsEnabled;
        public bool IsEnabled
        {
            get { return _IsEnabled; }
            set
            {
                if (_IsEnabled && !value)
                    Stop();
                else if (!_IsEnabled && value)
                    Start();
                else
                    return;


                _IsEnabled = value;
                RaisePropertyChanged("IsEnabled");
            }
        }

        public abstract void Start();
        public abstract void Stop();
        protected virtual void InvalidateConnection()
        {
            RaiseConnectionReset();
        }
        public abstract void Transmit(byte[] data);


    }
}

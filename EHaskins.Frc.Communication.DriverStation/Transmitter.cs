using System;
using System.ComponentModel;

namespace EHaskins.Frc.Communication.DriverStation
{
    public abstract class Transmitter
    {
        public Transmitter()
        {

        }
        protected void RaisePropertyChanged(string prop)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public abstract void Transmit(byte[] data, int teamNumber);
    }
}

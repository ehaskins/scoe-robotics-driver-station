using System;
using System.ComponentModel;

namespace EHaskins.Frc.Communication
{
    public class BindableBitField8 : BitField8, INotifyPropertyChanged
    {
        public BindableBitField8()
        {

        }
        public BindableBitField8(byte value)
            : base(value)
        {

        }

        public override byte RawValue
        {
            get
            {
                return base.RawValue;
            }
            set
            {
                base.RawValue = value;
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
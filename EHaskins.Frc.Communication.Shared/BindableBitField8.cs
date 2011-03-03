using System;
    using System.IO;
using System.ComponentModel;
using MiscUtil.IO;
using System.Diagnostics;

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
                Debug.WriteLine(property);
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }
    }
}
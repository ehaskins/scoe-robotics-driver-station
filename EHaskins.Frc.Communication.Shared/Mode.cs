using System;
using System.Text;
using System.Diagnostics;

namespace EHaskins.Frc.Communication
{
    public class Mode : BindableBitField8, ICloneable
    {
        private string[] Properties = { "IsFpgaChecksum", "IsCRioChecksum", "IsResync", "IsReserved", "IsAutonomous", "IsEnabled", "IsEStop", "IsReset" };
        public Mode() : this(84) { }
        public Mode(byte value)
            : base(value)
        {
            this.BitChanged += new BitChangedEventHandler(BitChangedHandler);
        }

        void BitChangedHandler(object sender, int bit)
        {
            RaisePropertyChanged(Properties[bit]);
        }

        object ICloneable.Clone()
        {
            return Clone();
        }

        public Mode Clone()
        {
            return new Mode(RawValue);
        }

        public bool IsFpgaChecksum
        {
            get { return this[0]; }
            set
            {
                this[0] = value;
            }
        }
        public bool IsCRioChecksum
        {
            get { return this[1]; }
            set
            {
                this[1] = value;
            }
        }
        public bool IsResync
        {
            get { return this[2]; }
            set
            {
                this[2] = value;
            }
        }
        public bool IsReserved
        {
            get { return this[3]; }
            set
            {
                this[3] = value;
            }
        }
        public bool IsAutonomous
        {
            get { return this[4]; }
            set
            {
                this[4] = value;
            }
        }
        public bool IsEnabled
        {
            get { return this[5]; }
            set
            {
                this[5] = value;
            }
        }
        public bool IsEStop
        {
            get { return !this[6]; }
            set
            {
                this[6] = !value;
            }
        }
        public bool IsReset
        {
            get { return this[7]; }
            set
            {
                this[7] = value;
            }
        }
    }
}

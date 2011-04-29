using System;

namespace EHaskins.Frc.Communication
{
    public class BindableBitField16 : BitField16
    {
        public BindableBitField16()
        {

        }
        public BindableBitField16(byte value)
            : base(value)
        {

        }

        public override ushort RawValue
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
    }
}

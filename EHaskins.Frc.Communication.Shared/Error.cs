using System;

namespace EHaskins.Frc.Communication
{
    public class Errors : BindableBitField8
    {
        public Errors()
        {

        }
        public Errors(byte value)
            : base(value)
        {

        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EHaskins.Frc.Communication
{
    public abstract class BitField8
    {
        public BitField8()
        {
            Length = 8;
        }
        public BitField8(byte value) : this()
        {
            this.RawValue = value;
        }

        private static byte GetMask(int index)
        {
            return (byte)Math.Pow(2, index);
        }
        public bool this[int index]
        {
            get
            {
                return (RawValue & GetMask(index)) == GetMask(index);
            }
            set
            {
                bool current = this[index];
                if (current != value)
                {
                    if (value)
                    {
                        RawValue += GetMask(index);
                    }
                    else
                    {
                        RawValue -= GetMask(index);
                    }
                }
            }
        }

        public virtual byte RawValue { get; set; }
        public int Length { get; protected set; }
    }
}
using System;

namespace EHaskins.Frc.Communication
{
    public abstract class BitField16
    {
        public BitField16()
        {
            Length = 16;
        }
        public BitField16(ushort value)
            : this()
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

        public virtual ushort RawValue { get; set; }
        public int Length { get; protected set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EHaskins.Frc.Communication.Filters
{
    abstract class PreFilter
    {
        public virtual void Begin() { }
        public abstract bool Process(byte[] data);
        public virtual void End() { }
    }
}

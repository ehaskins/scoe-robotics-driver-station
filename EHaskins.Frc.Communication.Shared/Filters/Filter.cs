using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EHaskins.Frc.Communication.Filters
{
    abstract class Filter
    {
        public virtual void Begin() { }
        public abstract bool Process(ControlData control, StatusData status);
        public virtual void End() { }
    }
}

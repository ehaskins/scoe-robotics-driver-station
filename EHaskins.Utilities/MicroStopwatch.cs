using System;
#if !NETMF
namespace MicroLibrary
{
    public class MicroStopwatch : System.Diagnostics.Stopwatch
    {
        double m_dMicroSecPerTick = 1000000D / Frequency;

        public MicroStopwatch()
        {
            if (!System.Diagnostics.Stopwatch.IsHighResolution)
                throw new Exception("On this system the high-resolution " +
                                    "performance counter is not available");
        }

        public long ElapsedMicroseconds
        {
            get {
                return (long)(ElapsedTicks * m_dMicroSecPerTick); 
            }
        }
    }
}
#endif
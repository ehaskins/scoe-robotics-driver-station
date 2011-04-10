using System;
#if NETMF
using Microsoft.SPOT;
#else
using System.Diagnostics;
#endif

namespace EHaskins.Utilities
{
    public class CrappyStopwatch
    {
        DateTime start;
        public CrappyStopwatch()
        {
            start = DateTime.Now;
        }

        public void PrintElapsed(string msg)
        {
            var now = DateTime.Now;
            var elapsed = now - start;
            Debug.Print(elapsed.ToString() + ":" + msg);
        }
    }
}

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
        static bool IsEnabled{get; set;}

        DateTime start;
        public CrappyStopwatch()
        {
            start = DateTime.Now;
        }

        public void PrintElapsed(string msg)
        {
            if (IsEnabled)
            {
                var now = DateTime.Now;
                var elapsed = now - start;
                Debug.Print(elapsed.ToString() + ":" + msg);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EHaskins.Utilities
{
    public static class IDisposableExtensions
    {
        public static void DisposeIfNotNull(this IDisposable obj)
        {
            if (obj != null)
            {
                obj.Dispose();
            }
        }
    }

}

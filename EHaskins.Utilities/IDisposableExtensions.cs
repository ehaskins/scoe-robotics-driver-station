using System;

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

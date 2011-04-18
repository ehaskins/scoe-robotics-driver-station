using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EHaskins.Utilities.NumericExtensions
{
    public static class MacAddressExtensions
    {
        public static string ToMacString(this byte[] data){
            if (data.Length != 6)
                throw new ArgumentException("MAC address data must be 6 bytes");
            return String.Format("{0:x2}:{1:x2}:{2:x2}:{3:x2}:{4:x2}:{5:x2}", data[0], data[1], data[2], data[3], data[4], data[5]);
        }
    }
}

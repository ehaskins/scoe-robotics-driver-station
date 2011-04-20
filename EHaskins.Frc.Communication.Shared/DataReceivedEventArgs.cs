using System;
#if NETMF
using Microsoft.SPOT;
#endif
namespace EHaskins.Frc.Communication
{
    public class DataReceivedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the DataReceivedEventArgs class.
        /// </summary>
        public DataReceivedEventArgs(byte[] data)
        {
            Data = data;
        }
        public byte[] Data { get; set; }
    }
}

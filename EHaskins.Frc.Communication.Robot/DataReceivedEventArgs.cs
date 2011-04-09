using System;
using System.Net;
using Microsoft.SPOT;

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

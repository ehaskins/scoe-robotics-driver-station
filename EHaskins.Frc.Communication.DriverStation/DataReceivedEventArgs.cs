using System;

namespace EHaskins.Frc.Communication.DriverStation
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

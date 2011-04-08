using System;
using MiscUtil.Conversion;

namespace EHaskins.Frc.Communication.RobotConfig
{
    public class RobotResponse
    {
        public RobotResponse(byte[] data)
        {
            var converter = new BigEndianBitConverter();
            DeviceID = converter.ToUInt16(data, 0);
            ResponseCode = (ResponseCode)data[2];
            for (int i = 3; i < data.Length; i++)
            {
                Payload[i - 3] = data[i];
            }
        }
        private ushort _DeviceID;
        public ushort DeviceID
        {
            get { return _DeviceID; }
            private set
            {
                _DeviceID = value;
            }
        }
        private ResponseCode _ResponseType;
        public ResponseCode ResponseCode
        {
            get { return _ResponseType; }
            private set
            {
                _ResponseType = value;
            }
        }

        private byte[] _Payload;
        public byte[] Payload
        {
            get { return _Payload; }
            private set
            {
                _Payload = value;
            }
        }

    }
}

using System;

namespace EHaskins.Frc.Communication.RobotConfig
{
    public enum ResponseCode : byte
    {
        Nck = 0,
        Ack = 1,
        MacAddress = 2,
        Data = 3
    }
}

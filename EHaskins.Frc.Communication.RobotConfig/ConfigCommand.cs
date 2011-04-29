using System;

namespace EHaskins.Frc.Communication.RobotConfig
{
    enum ConfigCommand : byte
    {
        Discover = 0x01,
        Read = 0x02,
        Write = 0x03,
        Erase = 0x04
    }
}

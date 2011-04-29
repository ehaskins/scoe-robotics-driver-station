using System;
using System.Net;
using MiscUtil.Conversion;

public static class FrcPacketUtils
{

    public static bool IsValidFrcPacket(this byte[] data)
    {
        uint dataCrc = EndianBitConverter.Big.ToUInt32(data, data.Length - 4);

        for (int i = data.Length - 4; i < data.Length; i++)
        {
            data[i] = 0;
        }
        //Dim calculatedCrc = BitConverter.ToUInt32(calulatedCrcBytes, 0)
        var calculatedCrc = Crc32.Compute(data);
        return dataCrc == calculatedCrc;
    }

    public static IPAddress GetIP(byte network, ushort teamNumber, byte hostNumber)
    {
        if (teamNumber > 9999)
            throw new ArgumentOutOfRangeException("teamNumber must be less than or equal to 9999.");
        var data = new byte[4];
        data[0] = network;
        data[1] = (byte)(teamNumber / 100);
        data[2] = (byte)(teamNumber % 100);
        data[3] = hostNumber;

        return new IPAddress(data);
    }
}
public enum Devices : byte
{
    Robot = 2,
    PC = 6,
    DS = 5,
    Router = 4,
    GameAdapter = 1
}
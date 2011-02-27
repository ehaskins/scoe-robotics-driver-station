using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Linq;
using System.Diagnostics;
using System.Net;
using MiscUtil.Conversion;

public static class FrcPacketUtils
{

    public static bool VerifyFrcCrc(byte[] data)
    {
        var tempData = new byte[data.Length];
        data.CopyTo(tempData, 0);
        uint dataCrc = 0;
        Crc32 crc = new Crc32();

        dataCrc = EndianBitConverter.Big.ToUInt32(tempData, tempData.Length - 4);

        for (int i = tempData.Length-4; i < tempData.Length; i++)
        {
            tempData[i] = 0;
        }
        //Dim calculatedCrc = BitConverter.ToUInt32(calulatedCrcBytes, 0)
        var calculatedCrc = Crc32.Compute(tempData);
        return dataCrc == calculatedCrc;
    }

    public static IPAddress GetIP(int teamNumber, Devices device)
    {
        dynamic num2 = teamNumber % 100;
        dynamic num1 = (teamNumber - num2) / 100;
        dynamic ipStr = string.Format("10.{0}.{1}.{2}", num1, num2, Convert.ToInt32(device));
        return IPAddress.Parse(ipStr);
    }
}
public enum Devices
{
    Robot = 2,
    PC = 6,
    DS = 5,
    Router = 4,
    GameAdapter = 1
}
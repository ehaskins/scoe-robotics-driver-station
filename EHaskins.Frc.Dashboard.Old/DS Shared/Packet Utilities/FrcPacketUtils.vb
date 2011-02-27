Imports System.Net
Imports MiscUtil.Conversion

Public Module FrcPacketUtils
    Public Function VerifyFrcCrc(ByVal data As Byte()) As Boolean
        'Dim calculatedCrc As UInteger = 0
        Dim dataCrc As UInteger = 0
        Dim crc As New Crc32()

        Dim dataCrcBytes As Byte() = data.Skip(data.Length - 4).Take(4).ToArray()
        dataCrc = EndianBitConverter.Big.ToUInt32(dataCrcBytes, 0)

        'remove CRC bytes from data before calculating CRC.
        Dim crcData(data.Length - 1) As Byte
        data.Take(data.Length - 4).ToArray.CopyTo(crcData, 0)

        Dim calulatedCrcBytes As Byte() = crc.ComputeHash(crcData)
        'Dim calculatedCrc = BitConverter.ToUInt32(calulatedCrcBytes, 0)
        Dim calculatedCrc = Crc32.Compute(crcData)
        Return dataCrc = calculatedCrc
    End Function

    Public Function GetIP(ByVal teamNumber As Integer, ByVal device As Devices) As IPAddress
        Dim num2 = teamNumber Mod 100
        Dim num1 = (teamNumber - num2) / 100
        Dim ipStr = String.Format("10.{0}.{1}.{2}", num1, num2, CInt(device))
        Return IPAddress.Parse(ipStr)
    End Function
End Module
Public Enum Devices
    Robot = 2
    PC = 6
    DS = 5
    Router = 4
    GameAdapter = 1
End Enum
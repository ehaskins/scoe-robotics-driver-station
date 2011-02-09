Imports System.Net

Public Module FrcPacketUtils
    'Public Function VerifyFrcCrc(ByVal data As Byte()) As Boolean
    '    Dim calculatedCrc As UInteger = 0
    '    Dim dataCrc As UInteger = 0
    '    Dim crc As New Crc32()

    '    dataCrc = BitConverter.ToUInt32(data.Skip(data.Length - 4).Take(4).ToArray(), 0)

    '    'remove CRC bytes from data before calculating CRC.
    '    Dim crcData(data.Length - 1) As Byte
    '    data.Take(data.Length - 4).ToArray.CopyTo(crcData, 0)

    '    calculatedCrc = BitConverter.ToUInt32(crc.ComputeHash(crcData), 0)

    '    Return dataCrc = calculatedCrc
    'End Function

    'Public Function VerifyFrcCrc(ByVal data As Byte()) As Boolean
    '    Dim calculatedCrc As UInteger = 0
    '    Dim dataCrc As UInteger = 0

    '    Using crc As New Crc32()
    '        dataCrc = BitConverter.ToUInt32(data, data.Length - 4)

    '        'remove CRC bytes from data before calculating CRC.
    '        For i As Integer = data.Length - 4 To data.Length
    '            data(i - 1) = 0
    '        Next

    '        calculatedCrc = BitConverter.ToUInt32(crc.ComputeHash(data), 0)

    '        Return dataCrc = calculatedCrc
    '    End Using
    'End Function

    Public Function VerifyFrcCrc(ByVal data As Byte()) As Boolean
        Dim calculatedCrc As UInteger = 0
        Dim dataCrc As UInteger = 0
        Dim crc As New Crc32()

        Dim dataCrcBytes As Byte() = data.Skip(data.Length - 4).Take(4).ToArray()
        dataCrc = BitConverter.ToUInt32(dataCrcBytes, 0)

        'remove CRC bytes from data before calculating CRC.
        Dim crcData(data.Length - 1) As Byte
        data.Take(data.Length - 4).ToArray.CopyTo(crcData, 0)

        Dim calulatedCrcBytes As Byte() = crc.ComputeHash(crcData)
        calculatedCrc = BitConverter.ToUInt32(calulatedCrcBytes, 0)

        Return dataCrc = calculatedCrc
    End Function

    Function GetIP(ByVal teamNumber As Integer, ByVal device As Devices) As IPAddress
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
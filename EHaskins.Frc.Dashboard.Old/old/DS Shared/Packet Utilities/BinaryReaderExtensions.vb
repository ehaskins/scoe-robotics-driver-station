Imports System.IO
Imports System.ComponentModel
Imports System.Collections.ObjectModel
Imports System.Runtime.CompilerServices
Imports MiscUtil.IO

Public Module BinaryReaderExtensions
    '<Extension()> _
    '    Public Function EReadUInt64(ByVal reader As BinaryReader) As UInt32
    '    Dim bytes = reader.ReadBytes(8)
    '    Return bytes(0) * 255 ^ 7 + bytes(1) * 255 ^ 6 + bytes(2) * 255 ^ 5 + _
    '            bytes(3) * 255 ^ 4 + bytes(4) * 255 ^ 3 + bytes(5) * 255 ^ 2 + _
    '            bytes(6) * 255 ^ 1 + bytes(7)
    'End Function

    '<Extension()> _
    'Public Function EReadUInt32(ByVal reader As BinaryReader) As UInt32
    '    Dim bytes = reader.ReadBytes(4)
    '    Return bytes(0) * 255 ^ 3 + bytes(1) * 255 ^ 2 + bytes(2) * 255 ^ 1 + bytes(3)
    'End Function

    '<Extension()> _
    'Public Function EReadUInt16(ByVal reader As BinaryReader) As UInt16
    '    Dim bytes = reader.ReadBytes(2)
    '    Return bytes(0) * 255 ^ 1 + bytes(1)
    'End Function

    '<Extension()> _
    'Public Function ReadTeamNumber(ByVal reader As BinaryReader) As Integer
    '    Dim temp1 = reader.ReadByte()
    '    Dim temp2 = reader.ReadByte()
    '    Return temp1 * 100 + temp2
    'End Function

    <Extension()> _
    Public Function ReadTeamNumber(ByVal reader As EndianBinaryReader) As Integer
        Dim temp1 = reader.ReadByte()
        Dim temp2 = reader.ReadByte()
        Return temp1 * 100 + temp2
    End Function

    <Extension()> _
    Public Function EReadBattery(ByVal reader As EndianBinaryReader) As Double
        ' battery voltage is transmitted follows: 0x12 0x17 is 12.17 volts.
        Return Convert.ToDouble(reader.ReadByte().ToString("x")) + (Convert.ToDouble(reader.ReadByte().ToString("x")) / 100.0)
    End Function
End Module

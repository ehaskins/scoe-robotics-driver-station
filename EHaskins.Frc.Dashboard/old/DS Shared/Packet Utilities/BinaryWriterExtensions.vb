Imports System.IO
Imports System.Runtime.CompilerServices

Public Module BinaryWriterExtensions
    <Extension()> _
    Public Sub EWrite(ByVal writer As BinaryWriter, ByVal value As UShort)
        writer.Write(BitConverter.GetBytes(value).Reverse().ToArray())
    End Sub

    <Extension()> _
    Public Sub EWrite(ByVal writer As BinaryWriter, ByVal value As ULong)
        writer.Write(BitConverter.GetBytes(value).Reverse().ToArray())
    End Sub

    <Extension()> _
    Public Sub EWrite(ByVal writer As BinaryWriter, ByVal value As UInteger)
        writer.Write(BitConverter.GetBytes(value).Reverse().ToArray())
    End Sub

    <Extension()> _
    Public Sub WriteTeamNumber(ByVal writer As BinaryWriter, ByVal teamNumber As Integer)
        Dim teamBytes(1) As Byte
        teamBytes(1) = teamNumber Mod 100
        teamBytes(0) = (teamNumber - teamBytes(1)) / 100
        writer.Write(teamBytes)
    End Sub
End Module

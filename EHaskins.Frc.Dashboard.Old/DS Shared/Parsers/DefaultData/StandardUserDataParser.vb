Imports System.IO
Imports System.ComponentModel
Imports System.Collections.ObjectModel
Imports System.Runtime.CompilerServices


Public Class StandardUserDataParser
    Implements IDashboardProcessor(Of StandardUserData)

    Public Sub New()
        MyBase.new()
        SetId(0, 0)

    End Sub

    Private _processorId As Integer = 0
    Private _processorOriginId As Integer = 0


    Public Function Parse(ByVal data() As Byte) As StandardUserData _
        Implements IDashboardProcessor(Of EHaskins.Frc.Dashboard.StandardUserData).Parse
        Return Update(data, New StandardUserData())

    End Function

    Public Function Update(ByVal data As Byte(), ByRef original As StandardUserData) As StandardUserData _
        Implements IDashboardProcessor(Of EHaskins.Frc.Dashboard.StandardUserData).Update
        Dim reader As New BinaryReader(New MemoryStream(data))

        original.SequenceNumber = reader.ReadByte()

        Dim printedString = New String(Text.Encoding.ASCII.GetChars(reader.ReadBytes(reader.EReadUInt32())))
        Dim strings = printedString.Split(vbNewLine)
        For Each s In strings
            If Not String.IsNullOrEmpty(s) Then
                original.PrintedStrings.Add(s)
            End If
        Next

        Dim errorString = New String(Text.Encoding.ASCII.GetChars(reader.ReadBytes(reader.EReadUInt32())))
        strings = errorString.Split(vbNewLine)
        For Each s In strings
            If Not String.IsNullOrEmpty(s) Then
                original.ErrorStrings.Add(s)
            End If
        Next

        Dim dataSize = reader.EReadUInt32()
        Dim userData As Byte() = reader.ReadBytes(dataSize)

        Return original
    End Function

    Protected Sub SetId(ByVal origin As Integer, ByVal Id As Integer)
        _processorId = Id
        _processorOriginId = origin
    End Sub
End Class

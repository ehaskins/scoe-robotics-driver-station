Imports System.Net.Sockets
Imports System.Threading
Imports System.Net
Imports System.IO
Imports System.ComponentModel
Imports MiscUtil.IO
Imports EHaskins.Frc.Communication

Public Interface IDashboardProcessor(Of T)
    Function Parse(ByVal data As Byte()) As T
    Function Update(ByVal data As Byte(), ByRef input As T) As T
End Interface
Public Class NonUserDataProcessor
    Dim dataGraph As NonUserData
    'Private Const TEAM_ROBOT_IP_ADDRESS As String = "10.11.3.2" 'Change this to match your robot's IP address

    ''' <summary>
    ''' Initializes a new instance of the NonUserDataProcessor class.
    ''' </summary>
    Public Sub New()
        dataGraph = New NonUserData()
    End Sub

    Public Function Parse(ByVal data As Byte()) As Byte()
        Dim reader As EndianBinaryReader = Nothing
        Try
            'reader = New BinaryReader(New MemoryStream(data))
            reader = New MiscUtil.IO.EndianBinaryReader(New MiscUtil.Conversion.BigEndianBitConverter(), New MemoryStream(data))

            dataGraph.PacketNumber = reader.ReadUInt16()
            dataGraph.DigitalIns = New BindableBitField8(reader.ReadByte())
            dataGraph.DigitalOuts = New BindableBitField8(reader.ReadByte())
            dataGraph.Battery = reader.EReadBattery()

            dataGraph.Status = Nothing 'TODO: BAD!
            dataGraph.Errors = New Errors(reader.ReadByte())

            dataGraph.TeamNumber = reader.ReadTeamNumber()

            Dim versionBytes As Byte() = reader.ReadBytes(8)
            dataGraph.DSVersion = Text.Encoding.ASCII.GetString(versionBytes)

            dataGraph.UnusedBuffer1 = reader.ReadUInt32()
            dataGraph.UnusedBuffer2 = reader.ReadUInt16()

            'I believe this is only important for robot-to-DS communication, but haven't confirmed.
            dataGraph.ReplyPacketNumber = reader.ReadUInt16()

            Dim userData = reader.ReadBytes(984)
            dataGraph.UserData = userData
            'ProcessUserData(UserDataBytes)
            'TODO: Confirm checksum, last 8 bytes.
            Return userData
            reader.Close()

            Return data
        Catch ex As Exception
            'TODO: Add exception handling logic.
            Throw
        Finally
            If reader IsNot Nothing Then
                reader.Close()
            End If
        End Try
    End Function
    'Public Overridable Function Parse(ByVal data As Byte()) As NonUserData Implements IDashboardProcessor(Of EHaskins.Frc.Dashboard.NonUserData).Parse
    '    Return Update(data, New NonUserData())
    'End Function
    'Public Overridable Function Update(ByVal data() As Byte, ByRef original As NonUserData) As NonUserData Implements IDashboardProcessor(Of EHaskins.Frc.Dashboard.NonUserData).Update
    '    Dim reader As BinaryReader = Nothing
    '    Try
    '        reader = New BinaryReader(New MemoryStream(data))
    '        If data.Length = DSPACKET_LENGTH Then


    '            original.PacketNumber = reader.ReadUInt16()
    '            original.DigitalIns = New DsInputs(reader.ReadByte())
    '            original.DigitalOuts = New DsOutputs(reader.ReadByte())
    '            original.Battery = reader.EReadBattery()

    '            original.Status = New RobotStatus(reader.ReadByte())
    '            original.Errors = New RobotError(reader.ReadByte())

    '            original.TeamNumber = reader.ReadTeamNumber()

    '            original.DSVersion = Text.Encoding.ASCII.GetString(reader.ReadBytes(8))

    '            original.UnusedBuffer1 = reader.ReadUInt32()
    '            original.UnusedBuffer2 = reader.ReadUInt16()

    '            'I believe this is only important for robot-to-DS communication, but haven't confirmed.
    '            original.ReplyPacketNumber = reader.ReadUInt16()

    '            Dim userData = reader.ReadBytes(984)
    '            original.UserData = userData
    '            'ProcessUserData(UserDataBytes)
    '            'TODO: Confirm checksum, last 8 bytes.
    '            reader.Close()
    '        Else
    '            Debug.Print("Invalid DS data received, ignoring")
    '        End If
    '        Return original
    '    Catch ex As Exception
    '        'TODO: Add exception handling logic.
    '        Throw
    '    Finally
    '        If reader IsNot Nothing Then
    '            reader.Close()
    '        End If
    '    End Try
    'End Function
    Public Property DataObject() As NonUserData
        Get
            Return dataGraph
        End Get
        Set(ByVal value As NonUserData)
            dataGraph = value
        End Set
    End Property



End Class

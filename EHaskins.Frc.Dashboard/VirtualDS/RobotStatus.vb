Imports System.IO

Public Class RobotStatus

    Private _controlData As StatusData
    Private _isValid As Boolean
    Private _replyId As UShort
    Private _robotMac As Byte() = {&H0, &H80, &H2F, &H11, &H4D, &HAC}
    Private _teamNumber As Integer
    Dim _batteryVoltage As Double
    Dim _dsOutputs As Byte = 0
    Dim _fpgaVersion As String = "06300800"
    Dim _codeRunning As Boolean

    Public Sub New()

    End Sub
    Public Sub New(ByVal data As Byte())
        Me.Parse(data)
    End Sub

    Public Sub Parse(ByVal data As Byte())
        Dim reader As New MiscUtil.IO.EndianBinaryReader(New MiscUtil.Conversion.BigEndianBitConverter(), New MemoryStream(data))

        reader.ReadByte()
        Dim batteryBytes = reader.ReadBytes(2)
        BatteryVoltage = Convert.ToDecimal(batteryBytes(0).ToString("x")) + (Convert.ToDecimal(batteryBytes(1).ToString("x")) / 100.0)

        reader.ReadBytes(5)

        TeamNumber = reader.ReadTeamNumber()
        RobotMac = reader.ReadBytes(6)

        reader.ReadBytes(14)

        ReplyId = reader.ReadUInt16()

        reader.ReadBytes(Configuration.UserControlDataSize + 4)

        IsValid = VerifyFrcCrc(data)
    End Sub

    Public Function CreateStatusPacket() As Byte()
        Dim data As Byte()
        Using stream As New MemoryStream()
            Dim writer As New MiscUtil.IO.EndianBinaryWriter(New MiscUtil.Conversion.BigEndianBitConverter(), stream, Text.Encoding.ASCII)
            writer.Write(ControlData.GetBytes())

            writer.Write(GetBatteryBytes())
            writer.Write(DsOutputs)
            Dim pad(3) As Byte '4 bytes
            writer.Write(pad)
            writer.Write(CType(TeamNumber, UInt16))
            writer.Write(RobotMac)
            writer.Write(Text.Encoding.ASCII.GetBytes(FpgaVersion))
            Dim pad2(5) As Byte '6 bytes
            writer.Write(pad2)
            writer.Write(CUShort(ReplyId))
            Dim paddingLength = 1024
            paddingLength -= writer.BaseStream.Position
            Dim padding(paddingLength - 1) As Byte
            writer.Write(padding)
            'TODO: Figure out why CRC calc is not being accepted on robot.
            Dim crcData = stream.ToArray()
            stream.Position -= 4
            writer.Write((New Crc32()).ComputeHash(crcData))
            data = stream.ToArray()
            writer.Close()
        End Using

        Debug.Assert(VerifyFrcCrc(data))
        Return data
    End Function

    Private Function GetBatteryBytes() As Byte()
        Dim batteryBytes As Byte()
        If CodeRunning Then
            Dim tempV = BatteryVoltage

            If tempV > 256 Then
                tempV = 256
            End If

            Dim int = Byte.Parse(CByte(tempV), Globalization.NumberStyles.HexNumber)
            Dim frac = Byte.Parse(CByte((tempV - CByte(tempV)) * 100), Globalization.NumberStyles.HexNumber)

            batteryBytes = New Byte() {int, frac}
        Else
            batteryBytes = New Byte() {&H37, &H37}
        End If
        Return batteryBytes
    End Function

    Public Property CodeRunning() As Boolean
        Get
            Return _codeRunning
        End Get
        Set(ByVal value As Boolean)
            _codeRunning = value
        End Set
    End Property
    Public Property ControlData() As StatusData
        Get
            Return _controlData
        End Get
        Set(ByVal value As StatusData)
            _controlData = value
        End Set
    End Property
    Public Property BatteryVoltage() As Double
        Get
            Return _batteryVoltage
        End Get
        Set(ByVal value As Double)
            _batteryVoltage = value
        End Set
    End Property
    Public Property DsOutputs() As Byte
        Get
            Return _dsOutputs
        End Get
        Set(ByVal value As Byte)
            _dsOutputs = value
        End Set
    End Property
    Public Property FpgaVersion() As String
        Get
            Return _fpgaVersion
        End Get
        Set(ByVal value As String)
            _fpgaVersion = value
        End Set
    End Property
    Public Property IsValid() As Boolean
        Get
            Return _isValid
        End Get
        Set(ByVal value As Boolean)
            _isValid = value
        End Set
    End Property
    Public Property ReplyId() As Integer
        Get
            Return _replyId
        End Get
        Set(ByVal value As Integer)
            If _replyId = value Then
                Return
            End If
            _replyId = value
        End Set
    End Property
    Public Property RobotMac() As Byte()
        Get
            Return _robotMac
        End Get
        Set(ByVal value As Byte())
            _robotMac = value
        End Set
    End Property
    Public Property TeamNumber() As Integer
        Get
            Return _teamNumber
        End Get
        Set(ByVal value As Integer)
            If _teamNumber = value Then
                Return
            End If
            _teamNumber = value
        End Set
    End Property
End Class

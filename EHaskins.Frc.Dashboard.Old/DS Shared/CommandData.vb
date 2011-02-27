Imports System.IO
Imports EHaskins.Frc.Communication
Imports System.ComponentModel
Imports System.Collections.ObjectModel

Public Class CommandData
    Implements ICloneable
    Implements INotifyPropertyChanged

    Public Sub New(ByVal data As Byte(), ByVal userControlDataSize As Integer)
        Me.New(0, userControlDataSize)
        Me.IsValid = False

        Parse(data)
    End Sub

    Public Sub New(ByVal packetIndex As Integer, ByVal userControlDataSize As Integer)
        Me.PacketId = packetIndex
        Me.UserControlDataLength = userControlDataSize
        Mode = New Mode()
        DsInputs = New BindableBitField8(0)
        Dim sticks(3) As Joystick
        For i = 0 To 3
            sticks(i) = New Joystick()
        Next
        Joysticks = sticks
        Version = New DSVersion()
        Dim inputs(3) As UInt16
        AnalogInputs = inputs
    End Sub


    Private _packetId As ObservableCollection(Of UInt16)
    Private _controlData As Mode
    Private _dsInputs As BindableBitField8
    Private _teamNumber As Integer
    Private _alliance As Alliance = Alliance.Red
    Private _position As Byte = 2

    Private _joysticks As ObservableCollection(Of Joystick)
    Private _analogInputs As ObservableCollection(Of UInt16)

    Private _cRioChecksum As UInt64 = 3472328296227680304
    Private _fpgaChecksum0 As UInt32 = 1179010630
    Private _fpgaChecksum1 As UInt32 = 1179010630
    Private _fpgaChecksum2 As UInt32 = 1179010630
    Private _fpgaChecksum3 As UInt32 = 1179010630

    Private _version As DSVersion

    Private _isValid As Boolean

    Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged


    Public Function Clone() As Object Implements System.ICloneable.Clone
        Dim out As New CommandData(Me.PacketId, UserControlDataLength)
        out.Alliance = Alliance
        out.AnalogInputs = AnalogInputs
        out.Mode = Mode
        out.CRioChecksum = CRioChecksum
        out.DsInputs = DsInputs
        out.FpgaChecksum0 = FpgaChecksum0
        out.FpgaChecksum1 = FpgaChecksum1
        out.FpgaChecksum2 = FpgaChecksum2
        out.FpgaChecksum3 = FpgaChecksum3
        out.Joysticks = Joysticks.Clone()
        out.PacketId = PacketId
        out.Position = Position
        out.TeamNumber = TeamNumber
        out.Version = Version
        Return out
    End Function

    Public Function GetNextPacket()
        Dim r As CommandData = Me.Clone()
        r.PacketId += 1
        Return r
    End Function

    Private Sub Parse(ByVal data As Byte())
        Dim stream As New MemoryStream(1024)
        Dim reader = New MiscUtil.IO.EndianBinaryReader(New MiscUtil.Conversion.BigEndianBitConverter(), New MemoryStream(data))
        PacketId = reader.ReadUInt16()

        Mode = New Mode(reader.ReadByte())
        DsInputs = New BindableBitField8(reader.ReadByte())
        TeamNumber = reader.ReadUInt16()
        Alliance = reader.ReadByte()
        Position = reader.ReadByte() - 48
        For i = 0 To 3
            Joysticks(i).Parse(reader)
        Next
        For i = 0 To 3
            AnalogInputs(i) = reader.ReadUInt16()
        Next
        CRioChecksum = reader.ReadUInt64()
        FpgaChecksum0 = reader.ReadUInt32()
        FpgaChecksum1 = reader.ReadUInt32()
        FpgaChecksum2 = reader.ReadUInt32()
        FpgaChecksum3 = reader.ReadUInt32()

        Version = New DSVersion(reader.ReadBytes(8))

        UserControlData = reader.ReadBytes(UserControlDataLength)

        Me.IsValid = VerifyFrcCrc(data)
    End Sub
    Public Function GetBytes() As Byte()
        Try
            Dim data As Byte()
            Using stream As MemoryStream = New MemoryStream()
                Dim writer As New MiscUtil.IO.EndianBinaryWriter(New MiscUtil.Conversion.BigEndianBitConverter(), stream, Text.Encoding.ASCII)
                writer.Write(CUShort(PacketId))
                writer.Write(Mode.RawValue)
                writer.Write(DsInputs.RawValue)
                writer.Write(CType(TeamNumber, UInt16))
                writer.Write(CByte(Alliance))
                writer.Write(CByte(48 + Position))
                For Each joystick In Joysticks
                    Dim joystickData As Byte() = joystick.GetBytes()
                    writer.Write(joystickData)
                Next
                For Each input As UInt16 In AnalogInputs
                    writer.Write(input)
                Next
                writer.Write(CRioChecksum)
                writer.Write(FpgaChecksum0)
                writer.Write(FpgaChecksum1)
                writer.Write(FpgaChecksum2)
                writer.Write(FpgaChecksum3)
                writer.Write(Version.GetBytes())

                Dim length As Integer
                If UserControlData Is Nothing Then
                    length = 0
                Else
                    length = If(UserControlData.Length <= UserControlDataLength, UserControlData.Length, UserControlDataLength)
                    writer.Write(UserControlData, 0, length)
                End If

                Dim paddingLength = (UserControlDataLength - length) + 8
                Dim padding(paddingLength - 1) As Byte
                writer.Write(padding)

                Dim crcData = stream.ToArray()
                stream.Position -= 4
                writer.Write((New Crc32()).ComputeHash(crcData))


                data = stream.ToArray()
                writer.Close()
            End Using

            Debug.Assert(VerifyFrcCrc(data))
            Return data
        Catch ex As Exception
            Throw
        End Try
    End Function

    Dim _userControlDataLength As Integer
    Public Property UserControlDataLength() As Integer
        Get
            Return _userControlDataLength
        End Get
        Set(ByVal value As Integer)
            If value <> _userControlDataLength Then
                _userControlDataLength = value
                RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs("UserControlDataLength"))
            End If
        End Set
    End Property

    Dim _userControlData As Byte()
    Public Property UserControlData() As Byte()
        Get
            Return _userControlData
        End Get
        Set(ByVal value As Byte())
            _userControlData = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs("UserControlData"))
        End Set
    End Property



    Public Property IsValid() As Boolean
        Get
            Return _isValid
        End Get
        Set(ByVal value As Boolean)
            If value <> _isValid Then
                _isValid = value
                RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs("IsValid"))
            End If
        End Set
    End Property

    Public Property Alliance() As Alliance
        Get
            Return _alliance
        End Get
        Set(ByVal value As Alliance)
            If value <> _alliance Then
                _alliance = value
                RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs("Alliance"))
            End If
        End Set
    End Property

    Public Property AnalogInputs() As UInt16()
        Get
            Return _analogInputs
        End Get
        Set(ByVal value As UInt16())
            If value <> _analogInputs Then
                _analogInputs = value
                RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs("AnalogInputs"))
            End If
        End Set
    End Property

    Public Property Mode() As Mode
        Get
            Return _controlData
        End Get
        Set(ByVal value As Mode)
            If value <> _controlData Then
                _controlData = value
                RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs("Mode"))
            End If
        End Set
    End Property

    Public Property CRioChecksum() As UInt64
        Get
            Return _cRioChecksum
        End Get
        Set(ByVal value As UInt64)
            If value <> _cRioChecksum Then
                _cRioChecksum = value
                RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs("CRioChecksum"))
            End If
        End Set
    End Property

    Public Property DsInputs() As BindableBitField8
        Get
            Return _dsInputs
        End Get
        Set(ByVal value As BindableBitField8)
            If value <> _dsInputs Then
                _dsInputs = value
                RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs("DsInputs"))
            End If
        End Set
    End Property

    Public Property FpgaChecksum0() As UInt32
        Get
            Return _fpgaChecksum0
        End Get
        Set(ByVal value As UInt32)
            If value <> _fpgaChecksum0 Then
                _fpgaChecksum0 = value
                RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs("FpgaChecksum0"))
            End If
        End Set
    End Property

    Public Property FpgaChecksum1() As UInt32
        Get
            Return _fpgaChecksum1
        End Get
        Set(ByVal value As UInt32)
            If value <> _fpgaChecksum1 Then
                _fpgaChecksum1 = value
                RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs("FpgaChecksum1"))
            End If
        End Set
    End Property

    Public Property FpgaChecksum2() As UInt32
        Get
            Return _fpgaChecksum2
        End Get
        Set(ByVal value As UInt32)
            If value <> _fpgaChecksum2 Then
                _fpgaChecksum2 = value
                RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs("FpgaChecksum2"))
            End If
        End Set
    End Property

    Public Property FpgaChecksum3() As UInt32
        Get
            Return _fpgaChecksum3
        End Get
        Set(ByVal value As UInt32)
            If value <> _fpgaChecksum3 Then
                _fpgaChecksum3 = value
                RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs("FpgaChecksum3"))
            End If
        End Set
    End Property

    Public Property Joysticks() As Joystick()
        Get
            Return _joysticks
        End Get
        Set(ByVal value As Joystick())
            If value <> _joysticks Then
                _joysticks = value
                RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs("Joysticks"))
            End If
        End Set
    End Property

    Public Property PacketId() As Integer
        Get
            Return _packetId
        End Get
        Set(ByVal value As Integer)
            If value <> _packetId Then
                _packetId = value
                RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs("PacketId"))
            End If
        End Set
    End Property

    Public Property Position() As Byte
        Get
            Return _position
        End Get
        Set(ByVal value As Byte)
            If value <> _position Then
                _position = value
                RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs("Position"))
            End If
        End Set
    End Property

    Public Property TeamNumber() As Integer
        Get
            Return _teamNumber
        End Get
        Set(ByVal value As Integer)
            If value <> _teamNumber Then
                _teamNumber = value
                RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs("TeamNumber"))
            End If
        End Set
    End Property

    Public Property Version() As DSVersion
        Get
            Return _version
        End Get
        Set(ByVal value As DSVersion)
            If value <> _version Then
                _version = value
                RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs("Version"))
            End If
        End Set
    End Property

End Class

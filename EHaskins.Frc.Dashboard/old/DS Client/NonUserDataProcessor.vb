Imports System.Net.Sockets
Imports System.Threading
Imports System.Net
Imports System.IO
Imports System.ComponentModel

Public Class NonUserDataProcessor
    Implements IDashboardDataProcessor
    Implements INotifyPropertyChanged


    Public Event PropertyChanged(ByVal sender As Object, ByVal e As PropertyChangedEventArgs) Implements System.ComponentModel.INotifyPropertyChanged.PropertyChanged

    Protected Sub RaisePropertyChanged(ByVal prop As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(prop))
    End Sub

    'Private Const TEAM_ROBOT_IP_ADDRESS As String = "10.11.3.2" 'Change this to match your robot's IP address
    Private Const DSPACKET_LENGTH As Integer = 1018


    Dim _packetNumber As UInt16
    Dim _teamNumber As UInt16
    Dim _digitalIns As DsInputs
    Dim _digitalOuts As DsOutputs
    Dim _battery As UInt16
    Dim _status As RobotStatus
    Dim _errors As RobotError

    Dim _DSVersion As DSVersion

    Dim _unusedBuffer1 As UInt32
    Dim _unusedBuffer2 As UInt16

    Dim _replyPacketNumber As UInt16

    Public Overridable Function Parse(ByVal data() As Byte) As Byte() Implements IDashboardDataProcessor.Parse
        Return MyClass.ShallowParse(data)
    End Function

    Public Overridable Function ShallowParse(ByVal data() As Byte) As Byte() Implements IDashboardDataProcessor.ShallowParse
        Dim reader As BinaryReader = Nothing
        Try
            reader = New BinaryReader(New MemoryStream(data))
            If data.Length = DSPACKET_LENGTH Then

                PacketNumber = reader.ReadUInt16()
                DigitalIns = New DsInputs(reader.ReadByte())
                DigitalOuts = New DsOutputs(reader.ReadByte())
                Battery = reader.ReadUInt16()

                Status = New RobotStatus(reader.ReadByte())
                _errors = New RobotError(reader.ReadByte())

                TeamNumber = reader.ReadTeamNumber()

                DSVersion = New DSVersion(reader.ReadBytes(8))

                UnusedBuffer1 = reader.ReadUInt32()
                UnusedBuffer2 = reader.ReadUInt16()

                'I believe this is only important for robot-to-DS communication, but haven't confirmed.
                ReplyPacketNumber = reader.ReadUInt16()

                Dim userData = reader.ReadBytes(984)
                'ProcessUserData(UserDataBytes)
                'TODO: Confirm checksum, last 8 bytes.
                reader.Close()
                Return userData
            Else
                Debug.Print("Invalid DS data received, ignoring")
            End If
        Catch ex As Exception
            Throw
        Finally
            If reader IsNot Nothing Then
                reader.Close()
            End If
        End Try
    End Function


    Public Property PacketNumber() As UInt16
        Get
            Return _packetNumber
        End Get
        Set(ByVal value As UInt16)
            If _packetNumber = value Then
                Return
            End If
            _packetNumber = value
            RaisePropertyChanged("PacketNumber")
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
            RaisePropertyChanged("TeamNumber")
        End Set
    End Property
    Public Property DigitalIns() As DsInputs
        Get
            Return _digitalIns
        End Get
        Set(ByVal value As DsInputs)
            If _digitalIns Is value Then
                Return
            End If
            _digitalIns = value
            RaisePropertyChanged("DigitalIns")
        End Set
    End Property
    Public Property DigitalOuts() As DsOutputs
        Get
            Return _digitalOuts
        End Get
        Set(ByVal value As DsOutputs)
            If _digitalOuts Is value Then
                Return
            End If
            _digitalOuts = value
            RaisePropertyChanged("DigitalOuts")
        End Set
    End Property
    Public Property Battery() As UInt16
        Get
            Return _battery
        End Get
        Set(ByVal value As UInt16)
            If _battery = value Then
                Return
            End If
            _battery = value
            RaisePropertyChanged("Battery")
        End Set
    End Property
    Public Property Status() As RobotStatus
        Get
            Return _status
        End Get
        Set(ByVal value As RobotStatus)
            If _status Is value Then
                Return
            End If
            _status = value
            RaisePropertyChanged("Status")
        End Set
    End Property
    Public Property Errors() As RobotError
        Get
            Return _errors
        End Get
        Set(ByVal value As RobotError)
            If _errors Is value Then
                Return
            End If
            _errors = value
            RaisePropertyChanged("Errors")
        End Set
    End Property
    Public Property DSVersion() As DSVersion
        Get
            Return _DSVersion
        End Get
        Set(ByVal value As DSVersion)
            If _DSVersion Is value Then
                Return
            End If
            _DSVersion = value
            RaisePropertyChanged("DSVersion")
        End Set
    End Property
    Public Property UnusedBuffer1() As UInt32
        Get
            Return _unusedBuffer1
        End Get
        Set(ByVal value As UInt32)
            If _unusedBuffer1 = value Then
                Return
            End If
            _unusedBuffer1 = value
            RaisePropertyChanged("UnusedBuffer1")
        End Set
    End Property
    Public Property UnusedBuffer2() As UInt16
        Get
            Return _unusedBuffer2
        End Get
        Set(ByVal value As UInt16)
            If _unusedBuffer2 = value Then
                Return
            End If
            _unusedBuffer2 = value
            RaisePropertyChanged("UnsedBuffer2")
        End Set
    End Property
    Public Property ReplyPacketNumber() As UInt16
        Get
            Return _replyPacketNumber
        End Get
        Set(ByVal value As UInt16)
            If _replyPacketNumber = value Then
                Return
            End If
            _replyPacketNumber = value
            RaisePropertyChanged("ReplyPacketNumber")
        End Set
    End Property
End Class

Imports System.ComponentModel

Public Class NonUserData
    Implements INotifyPropertyChanged


    Public Event PropertyChanged(ByVal sender As Object, ByVal e As PropertyChangedEventArgs) Implements System.ComponentModel.INotifyPropertyChanged.PropertyChanged

    Protected Sub RaisePropertyChanged(ByVal prop As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(prop))
    End Sub

    Dim _packetNumber As UInt16
    Dim _teamNumber As UInt16
    Dim _digitalIns As DsInputs
    Dim _digitalOuts As DsOutputs
    Dim _battery As Double
    Dim _status As StatusData
    Dim _errors As Errors
    Dim _DSVersion As String

    Dim _unusedBuffer1 As UInt32
    Dim _unusedBuffer2 As UInt16

    Dim _replyPacketNumber As UInt16

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
    Public Property Battery() As Double
        Get
            Return _battery
        End Get
        Set(ByVal value As Double)
            If _battery = value Then
                Return
            End If
            _battery = value
            RaisePropertyChanged("Battery")
        End Set
    End Property
    Public Property Status() As StatusData
        Get
            Return _status
        End Get
        Set(ByVal value As StatusData)
            If _status Is value Then
                Return
            End If
            _status = value
            RaisePropertyChanged("Status")
        End Set
    End Property
    Public Property Errors() As Errors
        Get
            Return _errors
        End Get
        Set(ByVal value As Errors)
            If _errors Is value Then
                Return
            End If
            _errors = value
            RaisePropertyChanged("Errors")
        End Set
    End Property
    Public Property DSVersion() As String
        Get
            Return _DSVersion
        End Get
        Set(ByVal value As String)
            If _DSVersion = value Then
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

    Private _userData As Byte()
    Public Property UserData() As Byte()
        Get
            Return _userData
        End Get
        Set(ByVal value As Byte())
            _userData = value

            RaisePropertyChanged("UserData")
        End Set
    End Property
End Class

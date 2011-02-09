Imports System.ComponentModel

Public Class RobotControlData
    Implements INotifyPropertyChanged
    'TODO: See if any of this works. :)
    Dim _raw As Byte

    Public Sub New()
    End Sub
    Public Sub New(ByVal data As Byte)
        RawData = data
    End Sub
    Public Property RawData() As Byte
        Get
            Return _raw
        End Get
        Set(ByVal value As Byte)
            If _raw = value Then
                Return
            End If
            _raw = value
            RaisePropertyChanged("RawData")
        End Set
    End Property

    Public Property Reset() As Boolean
        Get
            Return GetBit(RawData, 7)
        End Get
        Set(ByVal value As Boolean)
            RawData = SetBit(RawData, 7, value)
        End Set
    End Property
    Public Property Enabled() As Boolean
        Get
            Return GetBit(RawData, 5)
        End Get
        Set(ByVal value As Boolean)
            RawData = SetBit(RawData, 5, value)
        End Set
    End Property
    Public Property EStop() As Boolean
        Get
            Return Not GetBit(RawData, 6)
        End Get
        Set(ByVal value As Boolean)
            RawData = SetBit(RawData, 6, Not value)
        End Set
    End Property
    Public Property Autonomous() As Boolean
        Get
            Return GetBit(RawData, 4)
        End Get
        Set(ByVal value As Boolean)
            RawData = SetBit(RawData, 4, value)
        End Set
    End Property
    Public Property Resync() As Boolean
        Get
            Return GetBit(RawData, 2)
        End Get
        Set(ByVal value As Boolean)
            RawData = SetBit(RawData, 2, value)
        End Set
    End Property
    Public Property CRioCheckSum() As Boolean
        Get
            Return GetBit(RawData, 1)
        End Get
        Set(ByVal value As Boolean)
            RawData = SetBit(RawData, 1, value)
        End Set
    End Property
    Public Property FpgaCheckSum() As Boolean
        Get
            Return GetBit(RawData, 0)
        End Get
        Set(ByVal value As Boolean)
            RawData = SetBit(RawData, 0, value)
        End Set
    End Property

    Function GetBit(ByVal input As Byte, ByVal bit As Integer) As Boolean
        Dim b = input
        Dim bitOut As Integer

        For i = 0 To bit
            bitOut = b Mod 2
            b -= bitOut
            b /= 2
        Next

        Return Not bitOut = 0
    End Function

    Function SetBit(ByVal input As Byte, ByVal bit As Integer, ByVal value As Boolean) As Byte
        Dim bits(7) As Integer
        Dim b = input
        Dim out As Byte = 0

        'read all bits
        For i = 0 To 7
            bits(i) = b Mod 2
            b -= bits(i)
            b /= 2
        Next

        If value Then
            bits(bit) = 1
        Else
            bits(bit) = 0
        End If

        For i = 7 To 0 Step -1
            out *= 2
            out += bits(i)
        Next

        Return out
    End Function

    Public Function Getbytes() As Byte
        Return _raw
    End Function
    Public Event PropertyChanged(ByVal sender As Object, ByVal e As System.ComponentModel.PropertyChangedEventArgs) Implements System.ComponentModel.INotifyPropertyChanged.PropertyChanged
    Protected Sub RaisePropertyChanged(ByVal PropertyChangedEventArgs As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(PropertyChangedEventArgs))
    End Sub

End Class

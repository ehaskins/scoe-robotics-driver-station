Imports System.IO
Imports System.ComponentModel
Imports EHaskins.Frc.Dashboard
Imports MiscUtil.IO

Public Class Joystick
    Implements INotifyPropertyChanged

    Public Sub New()
    End Sub
    Public Sub New(ByVal number As Integer)
        JoystickNumber = number
    End Sub

    Dim _joystickNumber As Integer
    Dim _analogInputs(6) As Double
    Dim _digitalInputs(16) As Boolean

    Public Sub Parse(ByVal reader As EndianBinaryReader)
        For i = 0 To 5
            Dim byteRead As Integer = reader.ReadByte()
            Dim value As Double = (byteRead - 128) / 128
            AnalogInputs(i) = value
        Next

        'TODO: Add button handling code
        Dim bytes = reader.ReadBytes(2)

        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs("AnalogInputs"))
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs("X"))
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs("Y"))
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs("Z"))
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs("Twist"))
    End Sub

    Public Function GetBytes() As Byte()
        Dim stream As MemoryStream = New MemoryStream()
        Dim writer As New BinaryWriter(stream)
        For i = 0 To 5
            writer.Write(CByte(128))
        Next
        'TODO: Add button handling code here.
        writer.Write(CByte(0))
        writer.Write(CByte(0))

        Dim data = stream.ToArray()
        writer.Close()

        Return data
    End Function
    Public Property AnalogInputs() As Double()
        Get
            Return _analogInputs
        End Get
        Set(ByVal value As Double())
            If Not _analogInputs Is value Then
                Return
            End If
            _analogInputs = value

            'RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs("AnalogInputs"))
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs("X"))
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs("Y"))
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs("Z"))
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs("Twist"))
        End Set
    End Property
    Public Property JoystickNumber() As Integer
        Get
            Return _joystickNumber
        End Get
        Set(ByVal value As Integer)
            If _joystickNumber = value Then
                Return
            End If
            _joystickNumber = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs("JoystickNumber"))
        End Set
    End Property
    Public Property DigitalInputs() As Boolean()
        Get
            Return _digitalInputs
        End Get
        Set(ByVal value As Boolean())
            If Not _digitalInputs Is value Then
                Return
            End If
            _digitalInputs = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs("DigitalInputs"))
        End Set
    End Property

    Public ReadOnly Property X() As Integer
        Get
            Return AnalogInputs(0)
        End Get
    End Property
    Public ReadOnly Property Y() As Integer
        Get
            Return AnalogInputs(1)
        End Get
    End Property
    Public ReadOnly Property Z() As Integer
        Get
            Return AnalogInputs(2)
        End Get
    End Property
    Public ReadOnly Property Twist() As Integer
        Get
            Return AnalogInputs(3)
        End Get
    End Property

    Public Event PropertyChanged(ByVal sender As Object, ByVal e As System.ComponentModel.PropertyChangedEventArgs) Implements System.ComponentModel.INotifyPropertyChanged.PropertyChanged
End Class
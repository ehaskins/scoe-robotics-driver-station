Imports System.ComponentModel

Public Class DsInputs
    Implements INotifyPropertyChanged

    Public Event PropertyChanged(ByVal sender As Object, ByVal e As System.ComponentModel.PropertyChangedEventArgs) Implements System.ComponentModel.INotifyPropertyChanged.PropertyChanged

    Protected Sub RaisePropertyChanged(ByVal prop As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(prop))
    End Sub

    Dim _inputs(8) As Boolean

    Public Sub New(ByVal data As Byte)
        For i = 0 To 7
            Dim temp = data Mod 2
            Inputs(i) = temp
            data -= temp
            data /= 2
        Next
    End Sub
    Public Property Inputs() As Boolean()
        Get
            Return _inputs
        End Get
        Set(ByVal value As Boolean())
            _inputs = value
            RaisePropertyChanged("Inputs")
        End Set
    End Property
    Public Function GetByte() As Byte
        Dim out As Byte
        For i = 0 To 7
            out *= 2
            If _inputs(i) = True Then
                out += 1
            End If
        Next
        Return out
    End Function

End Class

Public Class DsOutputs
    Implements INotifyPropertyChanged

    Public Event PropertyChanged(ByVal sender As Object, ByVal e As System.ComponentModel.PropertyChangedEventArgs) Implements System.ComponentModel.INotifyPropertyChanged.PropertyChanged

    Protected Sub RaisePropertyChanged(ByVal prop As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(prop))
    End Sub

    Dim _outputs(8) As Boolean

    Public Sub New(ByVal data As Byte)
        For i = 0 To 7
            Dim temp = data Mod 2
            Outputs(i) = temp
            data -= temp
            data /= 2
        Next
    End Sub
    Public Property Outputs() As Boolean()
        Get
            Return _outputs
        End Get
        Set(ByVal value As Boolean())
            _outputs = value
            RaisePropertyChanged("Outputs")
        End Set
    End Property

End Class
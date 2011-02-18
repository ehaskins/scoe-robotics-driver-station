Imports System.ComponentModel
Imports System.Text

Public Class DSVersion
    Implements INotifyPropertyChanged
    Dim _bytes As Byte()
    Public Event PropertyChanged(ByVal sender As Object, ByVal e As System.ComponentModel.PropertyChangedEventArgs) Implements System.ComponentModel.INotifyPropertyChanged.PropertyChanged

    Public Sub New()
        VersionString = "090210a1"
    End Sub
    Protected Sub RaisePropertyChanged(ByVal prop As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(prop))
    End Sub

    Public Sub New(ByVal data As Byte())
        If data.Length = 8 Then
            VersionString = Encoding.ASCII.GetString(data)
            RaisePropertyChanged("VersionString")
        Else
            Throw New ArgumentOutOfRangeException("Data must be exactly 8 bytes in length.")
        End If
    End Sub

    Private _versionString As String
    Public Property VersionString As String
        Get
            Return _versionString
        End Get
        Set(ByVal Value As String)
            If (_versionString = Value) Then Return
            Dim bytes = Encoding.ASCII.GetBytes(Value)
            If Value Is Nothing OrElse Not Value.Length = 8 OrElse bytes.Length = 8 Then
                Throw New ArgumentException("VersionString must be exactly 8 characters.")
            End If

            _bytes = bytes
            _versionString = Value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs("VersionString"))
            
        End Set
    End Property

    Public Function GetBytes() As Byte()
        Return _bytes
    End Function
End Class



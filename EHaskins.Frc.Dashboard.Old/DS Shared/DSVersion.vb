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
            _bytes = data
            RaisePropertyChanged("VersionString")
            'TODO: Correct version parsing
            'versionMonth1 = data(0)
            'versionMonth2 = data(1)
            'versionDay1 = data(2)
            'versionDay2 = data(3)
            'versionYear1 = data(4)
            'versionYear2 = data(5)
            'versionRev1 = data(6)
            'versionRev2 = data(7)
        Else
            Throw New ArgumentOutOfRangeException("Data must be exactly 8 bytes in length.")
        End If
    End Sub

    Public Property VersionString() As String
        Get
            Return Encoding.ASCII.GetString(_bytes)
        End Get
        Set(ByVal value As String)
            _bytes = Encoding.ASCII.GetBytes(value)
            RaisePropertyChanged("VersionString")
        End Set
    End Property


    Public Function GetBytes() As Byte()
        If _bytes IsNot Nothing Then
            Return _bytes
        Else
            Dim out(8) As Byte
            Return out
        End If
    End Function


    'Dim versionMonth1 As Byte
    'Dim versionMonth2 As Byte
    'Dim versionDay1 As Byte
    'Dim versionDay2 As Byte
    'Dim versionYear1 As Byte
    'Dim versionYear2 As Byte
    'Dim versionRev1 As Byte
    'Dim versionRev2 As Byte

    'Private _versionDate As DateTime
    'Private _versionRev As Integer


    'Public Property VersionDate() As DateTime
    '    Get
    '        Return _versionDate
    '    End Get
    '    Set(ByVal value As DateTime)
    '        If _versionDate = value Then
    '            Return
    '        End If
    '        _versionDate = value
    '        RaisePropertyChanged("VersionDate")
    '    End Set
    'End Property
    'Public Property VersionRev() As Integer
    '    Get
    '        Return _versionRev
    '    End Get
    '    Set(ByVal value As Integer)
    '        If _versionRev = value Then
    '            Return
    '        End If
    '        _versionRev = value
    '        RaisePropertyChanged("VersionRev")
    '    End Set
    'End Property
End Class



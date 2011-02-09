Imports System.Collections.ObjectModel
Imports System.ComponentModel

Public Class StandardUserData
    Implements INotifyPropertyChanged

    Public Event PropertyChanged(ByVal sender As Object, ByVal e As PropertyChangedEventArgs) Implements System.ComponentModel.INotifyPropertyChanged.PropertyChanged

    Protected Sub RaisePropertyChanged(ByVal prop As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(prop))
    End Sub

    Dim _sequenceNumber As Integer
    Dim _printedStrings As ObservableCollection(Of String)
    Dim _errorStrings As ObservableCollection(Of String)
    Dim _userData As Byte()

    Public Sub New()
        _printedStrings = New ObservableCollection(Of String)
        _errorStrings = New ObservableCollection(Of String)
    End Sub
    Public Property ErrorStrings() As ObservableCollection(Of String)
        Get
            Return _errorStrings
        End Get
        Set(ByVal value As ObservableCollection(Of String))
            If _errorStrings Is value Then
                Return
            End If
            _errorStrings = value
            RaisePropertyChanged("ErrorStrings")
        End Set
    End Property
    Public Property PrintedStrings() As ObservableCollection(Of String)
        Get
            Return _printedStrings
        End Get
        Set(ByVal value As ObservableCollection(Of String))
            If _printedStrings Is value Then
                Return
            End If
            _printedStrings = value
            RaisePropertyChanged("PrintedStrings")
        End Set
    End Property
    Public Property SequenceNumber() As Integer
        Get
            Return _sequenceNumber
        End Get
        Set(ByVal value As Integer)
            If _sequenceNumber = value Then
                Return
            End If
            _sequenceNumber = value
            RaisePropertyChanged("SequenceNumber")
        End Set
    End Property
    Public Overridable Property UserData() As Byte()
        Get
            Return _userData
        End Get
        Set(ByVal value As Byte())
            _userData = value
            RaisePropertyChanged("UserData")
        End Set
    End Property
End Class

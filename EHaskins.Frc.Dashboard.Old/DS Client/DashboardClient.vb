Imports System.Net.Sockets
Imports System.Threading
Imports System.Net
Imports System.IO
Imports System.ComponentModel
Imports EHaskins.Frc.Dashboard.Clients
Public Interface IDashboardClient
    Event NewDashboardData As EventHandler(Of NewDashboardDataEventArgs)
End Interface

Public Class DashboardClient
    Implements INotifyPropertyChanged, IDisposable, IDashboardClient, IRawDashboardProcessor

    Public Event NewDashboardData As EventHandler(Of NewDashboardDataEventArgs) Implements IDashboardClient.NewDashboardData

    Protected Sub RaiseNewDashboardData(ByVal data As Byte())
        RaiseEvent NewDashboardData(Me, New NewDashboardDataEventArgs(data))
    End Sub
    Private _invalidPacketCount As Integer = 0
    Private Const DS_TO_PC_LOCAL_PORT As Integer = 1165
    Private Const DS_TO_PC_REMOTE_PORT As Integer = 1170
    Private Const DSPACKET_LENGTH As Integer = 1018

    Dim _client As UdpClient
    Dim _dsEndpoint As EndPoint
    Dim _listening As Boolean
    Dim _updateThread As Thread

    Public Sub New()
    End Sub

    Public Event PropertyChanged(ByVal sender As Object, ByVal e As PropertyChangedEventArgs) Implements System.ComponentModel.INotifyPropertyChanged.PropertyChanged

    Protected Sub RaisePropertyChanged(ByVal prop As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(prop))
    End Sub

    'Dim _DSThread As Thread

    Public Property Listening() As Boolean
        Get
            Return _listening
        End Get
        Private Set(ByVal value As Boolean)
            If value = _listening Then
                Return
            End If
            _listening = value
            'If value Then
            '    BeginGetDataAsync()
            'Else
            '    EndGetDataAsync()
            'End If
        End Set
    End Property

    Sub BeginGetDataAsync()
        _listening = True
        _client = New UdpClient(DS_TO_PC_LOCAL_PORT)
        _client.BeginReceive(AddressOf Me.GetDataAsync, Nothing)
    End Sub

    Sub GetDataAsync(ByVal result As IAsyncResult)
        Try
            Dim endPoint As IPEndPoint = Nothing
            Dim bytes = _client.EndReceive(result, EndPoint)
            ParseBytes(bytes)
        Catch ex As Exception
            Debug.WriteLine(ex.Message)
            'Debug.Assert(False)
            Throw
        Finally
            If Listening Then
                _client.BeginReceive(AddressOf Me.GetDataAsync, Nothing)
            End If
        End Try
    End Sub

    Sub EndGetDataAsync()
        _listening = False
        _client.Close()
    End Sub

    Sub GetData()
        Dim client = New UdpClient(DS_TO_PC_LOCAL_PORT)
        Dim dsEndpoint = New IPEndPoint(IPAddress.Any, DS_TO_PC_REMOTE_PORT)
        While (Listening)
            ParseBytes(client.Receive(dsEndpoint))
        End While
    End Sub

    Public Sub BeginUpdating()
        Listening = True
        '_updateThread = New Thread(AddressOf Me.GetData)
        '_updateThread.Name = "Dashboard thread"
        '_updateThread.Start()
        BeginGetDataAsync()
    End Sub

    Sub ParseBytes(ByVal data As Byte())
        If data.Length = DSPACKET_LENGTH AndAlso VerifyFrcCrc(data) Then
            UserData = data
            RaiseNewDashboardData(data)
            InvalidPacketCount = 0
        Else
            InvalidPacketCount += 1
            Debug.Print("Invalid DS data received, ignoring")
        End If
    End Sub


    Public Property InvalidPacketCount() As Integer
        Get
            Return _invalidPacketCount
        End Get
        Set(ByVal value As Integer)
            If _invalidPacketCount = value Then
                Return
            End If
            _invalidPacketCount = value
            RaisePropertyChanged("InvalidPacketCount")
        End Set
    End Property

    Private _userData As byte()
    Public Property UserData() As byte()
        Get
            Return _userData
        End Get
        Set(ByVal Value As byte())
            _userData = Value
        End Set
    End Property

#Region "IDisposable"
    Private disposedValue As Boolean = False        ' To detect redundant calls

    ' IDisposable
    Protected Overridable Sub Dispose(ByVal disposing As Boolean)
        If Not Me.disposedValue Then
            If disposing Then
                _listening = False
                _client.Close()
            End If

            ' TODO: free your own state (unmanaged objects).
            ' TODO: set large fields to null.
        End If
        Me.disposedValue = True
    End Sub

#Region " IDisposable Support "
    ' This code added by Visual Basic to correctly implement the disposable pattern.
    Public Sub Dispose() Implements IDisposable.Dispose
        ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub
#End Region
#End Region

End Class

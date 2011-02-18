Imports System.Timers
Imports System.Windows
Imports System.Net.Sockets
Imports System.Net
Imports MicroLibrary

Public Class VirtualDS
    Implements IDisposable

    Dim _isOpen As Boolean
    Dim _transmitTimer As MicroTimer

    Dim _robotStatus As StatusData
    Dim _controlData As CommandData
    Dim _transmitClient As UdpClient
    Dim _receviceClient As UdpClient
    Dim _transmitEP As IPEndPoint
    Dim _receiveEP As IPEndPoint
    Dim _isSyncronized As Boolean = False

    Private _invalidPacketCount As Integer = 0
    Private _missedPacketCount As Integer = 0
    Private _totalInvalidPacketCount As Integer = 0
    Private _packetIndex As Integer = 0

    Public Sub New(ByVal teamNumber As Integer)
        _controlData = New CommandData(1)
        _controlData.Mode.RawData = 84

        _controlData.TeamNumber = teamNumber
    End Sub

    Public Sub Open(ByVal teamNumber As Integer, Optional ByVal transmitEP As EndPoint = Nothing, Optional ByVal receivePort As Integer = 1150)
        _transmitEP = If(transmitEP Is Nothing, New IPEndPoint(GetIP(teamNumber, Devices.Robot), 1110), transmitEP)

        _transmitClient = New UdpClient()

        _receviceClient = New UdpClient(receivePort)

        _isOpen = True
        _receviceClient.BeginReceive(AddressOf Me.ReceiveData, Nothing)

        _transmitTimer = New MicroTimer(20000)
        AddHandler _transmitTimer.Elapsed, AddressOf Me.SendData
        '_transmitTimer.AutoReset = True
        _transmitTimer.Start()
    End Sub

    Public Sub Close()
        If _isOpen AndAlso _transmitClient IsNot Nothing Then
            _transmitTimer.Stop()
        End If
    End Sub

    Private Sub CheckSafties()
        If _robotStatus Is Nothing OrElse (_robotStatus.ReplyId < CommandData.PacketId) Then
            CurrentInvalidPacketCount += 1
        End If
        If CurrentInvalidPacketCount > Configuration.InvalidPacketCountSafety Then
            'TODO: Raise event here.
            SafteyTriggered = True
        Else
            SafteyTriggered = False
        End If
    End Sub

    Private Sub UpdateMode()

        CommandData.PacketId += 1

        If SafteyTriggered Then
            CommandData.Mode.Enabled = False
            IsSyncronized = False
        End If

        If Not IsSyncronized Or CommandData.PacketId = Integer.MaxValue Then
            _controlData.Mode.Resync = True
        Else
            _controlData.Mode.Resync = False
        End If


    End Sub

    Public Sub SendData(ByVal sender As Object, ByVal e As MicroTimerEventArgs)
        '_transmitTimer.Enabled = False
        Try
            CheckSafties()
            UpdateMode()

            Dim sendData = _controlData.GetBytes()
            _transmitClient.Send(sendData, sendData.Length, _transmitEP)
        Catch ex As Exception
            Debug.WriteLine(ex.Message)
        End Try
        '_transmitTimer.Enabled = True
    End Sub

    Public Sub ReceiveData(ByVal ar As IAsyncResult)
        Try
            Dim endpoint As IPEndPoint = Nothing
            Dim bytes = _receviceClient.EndReceive(ar, endpoint)
            ParseBytes(bytes)
        Catch ex As Exception
            Throw
        End Try
    End Sub

    Private Sub ParseBytes(ByVal data As Byte())
        Try
            _robotStatus = New StatusData(data)

            If _robotStatus.IsValid Then
                CurrentInvalidPacketCount = 0
            Else
                CurrentInvalidPacketCount += 1
            End If
        Catch ex As Exception
            Debug.WriteLine(ex.Message)
            Debug.Assert(False)
            Throw
        Finally
            If _isOpen Then
                _receviceClient.BeginReceive(AddressOf Me.ReceiveData, Nothing)
            End If
        End Try
    End Sub

    Public Property IsSyncronized() As Boolean
        Get
            Return _isSyncronized
        End Get
        Set(ByVal value As Boolean)
            _isSyncronized = value
        End Set
    End Property

    Public ReadOnly Property CommandData() As CommandData
        Get
            Return _controlData
        End Get
    End Property

    Public Property TotalInvalidPacketCount() As Integer
        Get
            Return _totalInvalidPacketCount
        End Get
        Set(ByVal value As Integer)
            _totalInvalidPacketCount = value
        End Set
    End Property

    Public Property CurrentInvalidPacketCount() As Integer
        Get
            Return _invalidPacketCount
        End Get
        Set(ByVal value As Integer)
            _invalidPacketCount = value
        End Set
    End Property

    Private _safteyTriggered As Boolean
    Public Property SafteyTriggered() As Boolean
        Get
            Return _safteyTriggered
        End Get
        Set(ByVal value As Boolean)
            _safteyTriggered = value
        End Set
    End Property

#Region "IDisposable Support"
    Private disposedValue As Boolean ' To detect redundant calls

    ' IDisposable
    Protected Overridable Sub Dispose(ByVal disposing As Boolean)
        If Not Me.disposedValue Then
            If disposing Then
                ' TODO: dispose managed state (managed objects).
                Me.Close()
            End If

            ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
            ' TODO: set large fields to null.
        End If
        Me.disposedValue = True
    End Sub

    ' TODO: override Finalize() only if Dispose(ByVal disposing As Boolean) above has code to free unmanaged resources.
    'Protected Overrides Sub Finalize()
    '    ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
    '    Dispose(False)
    '    MyBase.Finalize()
    'End Sub

    ' This code added by Visual Basic to correctly implement the disposable pattern.
    Public Sub Dispose() Implements IDisposable.Dispose
        ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub
#End Region

End Class


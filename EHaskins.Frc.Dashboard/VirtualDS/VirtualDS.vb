Imports System.Timers
Imports System.Windows
Imports System.Net.Sockets
Imports System.Net

Public Class VirtualDS
    Dim _isOpen As Boolean
    Dim _transmitTimer As Timer

    Dim _robotStatus As RobotStatus
    Dim _controlData As ControlData
    Dim _transmitClient As UdpClient
    Dim _receviceClient As UdpClient
    Dim _transmitEP As IPEndPoint
    Dim _receiveEP As IPEndPoint
    Dim _isConnected As Boolean = False

    Private _invalidPacketCount As Integer = 0
    Private _missedPacketCount As Integer = 0
    Private _totalMissedPacketCount As Integer = 0
    Private _packetIndex As Integer = 0

    Public Sub New(ByVal teamNumber As Integer)
        _controlData = New ControlData(1)
        _controlData.ControlData.RawData = 84

        _controlData.TeamNumber = teamNumber
    End Sub

    Public Sub Open(ByVal teamNumber As Integer, Optional ByVal transmitEP As EndPoint = Nothing, Optional ByVal receivePort As Integer = 1150)
        _transmitEP = If(transmitEP Is Nothing, New IPEndPoint(GetIP(teamNumber, Devices.Robot), 1110), transmitEP)

        _transmitClient = New UdpClient()

        _receviceClient = New UdpClient(receivePort)

        _isOpen = True
        _receviceClient.BeginReceive(AddressOf Me.ReceiveData, Nothing)

        _transmitTimer = New Timer(20)
        AddHandler _transmitTimer.Elapsed, AddressOf Me.SendData
        _transmitTimer.AutoReset = True
        _transmitTimer.Start()
    End Sub

    Public Sub SendData(ByVal sender As Object, ByVal e As ElapsedEventArgs)
        '_transmitTimer.Enabled = False
        Try
            ControlData.PacketId += 1
            If MissedPacketCount > Configuration.MissedPacketCountSafety Or _
                InvalidPacketCount > Configuration.InvalidPacketCountSafety Then
                IsConnected = False
            Else
                IsConnected = True
            End If

            If Not IsConnected Then
                Debug.WriteLine("Not connected")
                _controlData.PacketId = 0
                _controlData.ControlData.Enabled = False
                _controlData.ControlData.Resync = False
                _controlData.ControlData.Reset = False
                _controlData.ControlData.Autonomous = False
                _controlData.ControlData.EStop = False
                _controlData.ControlData.CRioCheckSum = False
                _controlData.ControlData.FpgaCheckSum = False
            Else
                Debug.WriteLine("Connected")
                _controlData.ControlData.Resync = False
                _controlData.ControlData.Enabled = True
                _controlData.ControlData.Autonomous = True
            End If

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
            _robotStatus = New RobotStatus(data)
            Debug.WriteLine(_robotStatus.ReplyId)

            If _robotStatus.IsValid Then
                InvalidPacketCount = 0
            Else
                InvalidPacketCount += 1
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

    Public Property IsConnected() As Boolean
        Get
            Return _isConnected
        End Get
        Set(ByVal value As Boolean)
            _isConnected = value
        End Set
    End Property

    Public ReadOnly Property ControlData() As ControlData
        Get
            Return _controlData
        End Get
    End Property

    Public Property MissedPacketCount() As Integer
        Get
            Return _missedPacketCount
        End Get
        Set(ByVal value As Integer)
            _missedPacketCount = value
        End Set
    End Property

    Public Property TotalMissedPacketCount() As Integer
        Get
            Return _totalMissedPacketCount
        End Get
        Set(ByVal value As Integer)
            _totalMissedPacketCount = Value
        End Set
    End Property

    Public Property InvalidPacketCount() As Integer
        Get
            Return _invalidPacketCount
        End Get
        Set(ByVal value As Integer)
            _invalidPacketCount = value
        End Set
    End Property


End Class


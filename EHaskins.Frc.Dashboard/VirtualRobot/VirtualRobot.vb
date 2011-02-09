Imports System.Net.Sockets
Imports System.Net

Public Class VirtualRobot
    Dim _isOpen As Boolean

    Dim _controlData As ControlData
    Dim _status As RobotStatus

    Dim _transmitClient As UdpClient
    Dim _receviceClient As UdpClient
    Dim _isConnected As Boolean = False

    Private _invalidPacketCount As Integer = 0
    Private _missedPacketCount As Integer = 0
    Private _packetCount As Integer = 0
    Private _teamNumber As Integer = 1103

    Public Sub New(ByVal teamNumber As Integer)
        Start(teamNumber)
    End Sub

    Public Event NewDataReceived As EventHandler

    Private Sub Start(ByVal teamNumber As Integer)
        _status = New RobotStatus()

        Me.TeamNumber = teamNumber

        _status.TeamNumber = Me.TeamNumber

        _transmitClient = New UdpClient()

        _receviceClient = New UdpClient(Configuration.DsToRobotLocalPortNumber)
        _isOpen = True
        _receviceClient.BeginReceive(AddressOf Me.ReceiveData, Nothing)
    End Sub

    Private Sub SendReply(ByVal packet As ControlData, ByVal endpoint As EndPoint)
        If _status.ReplyId > packet.PacketId And Not packet.ControlData.Resync Then
            Return
        End If
        _status.ReplyId = packet.PacketId
        _status.ControlData = packet.ControlData.Clone()

        Dim sendData = _status.CreateStatusPacket()
        Dim ipep As IPEndPoint = DirectCast(endpoint, IPEndPoint)

        ipep.Port = 1150

        _transmitClient.Send(sendData, sendData.Length, ipep)
    End Sub

    Public Sub ReceiveData(ByVal ar As IAsyncResult)
        Try
            Dim endpoint As IPEndPoint = Nothing
            Dim bytes = _receviceClient.EndReceive(ar, endpoint)
            Dim packet = ParseBytes(bytes)
            If packet IsNot Nothing AndAlso packet.IsValid AndAlso packet.TeamNumber = Me.TeamNumber Then
                SendReply(packet, endpoint)
                RaiseEvent NewDataReceived(Me, Nothing)
            Else
                InvalidPacketCount += 1
            End If
        Catch ex As Exception
            Throw
        Finally
            If _isOpen Then
                _receviceClient.BeginReceive(AddressOf Me.ReceiveData, Nothing)
            End If
        End Try
    End Sub


    Private Function ParseBytes(ByVal data As Byte()) As ControlData
        Try
            _packetCount += 1
            Dim packet = New ControlData(data)
            ControlData = packet

            Debug.WriteLine(ControlData.PacketId)

            If Not ControlData.IsValid Then
                InvalidPacketCount += 1
            End If

            Return packet
        Catch ex As Exception
            Debug.WriteLine(ex.Message)
            Throw
        End Try
    End Function

    Public Property IsConnected() As Boolean
        Get
            Return _isConnected
        End Get
        Set(ByVal value As Boolean)
            _isConnected = value
        End Set
    End Property
    Public Property ControlData() As ControlData
        Get
            Return _controlData
        End Get
        Private Set(ByVal value As ControlData)
            _controlData = value
        End Set
    End Property
    Public ReadOnly Property Status() As RobotStatus
        Get
            Return _status
        End Get
    End Property

    Public Property InvalidPacketCount() As Integer
        Get
            Return _invalidPacketCount
        End Get
        Set(ByVal value As Integer)
            _invalidPacketCount = value
        End Set
    End Property

    Public Property TeamNumber() As Integer
        Get
            Return _teamNumber
        End Get
        Set(ByVal value As Integer)
            _teamNumber = value
        End Set
    End Property

End Class
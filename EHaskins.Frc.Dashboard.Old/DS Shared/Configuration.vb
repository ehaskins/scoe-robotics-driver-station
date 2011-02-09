Public Module Configuration
    Const DEFAULT_USER_CONTROL_DATA_SIZE As Integer = 936
    Const DEFAULT_INVALID_PACKET_SAFETY_LEVEL As Integer = 25
    Const DS_TO_ROBOT_LOCAL_PORT As Integer = 1115
    Const DS_TO_ROBOT_REMOTE_PORT As Integer = 1110
    Const ROBOT_TO_DS_LOCAL_PORT As Integer = 1150
    Const ROBOT_TO_DS_REMOTE_PORT As Integer = 1026

    Dim _userControlDataSize As Integer = DEFAULT_USER_CONTROL_DATA_SIZE
    Private _invalidPacketCountSafety As Integer = DEFAULT_INVALID_PACKET_SAFETY_LEVEL

    Private _dsToRobotRemotePortNumber As Integer = DS_TO_ROBOT_REMOTE_PORT
    Private _dsToRobotLocalPortNumber As Integer = DS_TO_ROBOT_REMOTE_PORT
    Private _robotToDsRemotePortNumber As Integer = ROBOT_TO_DS_LOCAL_PORT
    Private _robotToDsLocalPortNumber As Integer = ROBOT_TO_DS_LOCAL_PORT

    Public Property DsToRobotLocalPortNumber() As Integer
        Get
            Return _dsToRobotLocalPortNumber
        End Get
        Set(ByVal Value As Integer)
            _dsToRobotLocalPortNumber = Value
        End Set
    End Property
    Public Property DsToRobotRemotePortNumber() As Integer
        Get
            Return _dsToRobotRemotePortNumber
        End Get
        Set(ByVal Value As Integer)
            _dsToRobotRemotePortNumber = Value
        End Set
    End Property
    Public Property RobotToDsLocalPortNumber() As Integer
        Get
            Return _robotToDsLocalPortNumber
        End Get
        Set(ByVal Value As Integer)
            _robotToDsLocalPortNumber = Value
        End Set
    End Property
    Public Property RobotToDsRemotePortNumber() As Integer
        Get
            Return _robotToDsRemotePortNumber
        End Get
        Set(ByVal Value As Integer)
            _robotToDsRemotePortNumber = Value
        End Set
    End Property

    Public ReadOnly Property InvalidPacketCountSafety() As Integer
        Get
            Return _invalidPacketCountSafety
        End Get
    End Property

    Public Property UserControlDataSize() As Integer
        Get
            Return _userControlDataSize
        End Get
        Set(ByVal value As Integer)
            _userControlDataSize = value
        End Set
    End Property

End Module

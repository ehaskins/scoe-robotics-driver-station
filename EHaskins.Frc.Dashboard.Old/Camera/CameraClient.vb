Imports System.Drawing
Imports System.Net.Sockets
Imports System.IO
Imports System.Net
Imports System.Threading
Imports System.ComponentModel
Imports EHaskins.Utilities
Imports System.Windows.Media.Imaging
Imports MiscUtil.IO
Imports MiscUtil.Conversion
Public Class CameraClient
    Implements ICameraClient

    Private Const CAMERA_RECEIVE_TIMEOUT As Integer = 500

    Dim _updateThread As Thread
    Dim _listening As Boolean

    Dim _robotAddress As IPAddress
    Dim _robotPort As Integer = 1180
    Dim _teamNumber As Integer

    Dim client As New TcpClient()

    Public Sub New()
    End Sub

    Public Sub New(ByVal teamNumber As Integer)
        Me.TeamNumber = teamNumber
    End Sub

    Public Event NewImageData As EventHandler(Of NewImageDataEventArgs) Implements ICameraClient.NewImageData

    Private Sub RaiseNewImageData(ByVal imageData As Byte())
        RaiseEvent NewImageData(Me, New NewImageDataEventArgs(imageData))
    End Sub
    Public Property Listening() As Boolean
        Get
            Return _listening
        End Get
        Private Set(ByVal value As Boolean)
            _listening = value
            'RaisePropertyChanged("Listening")
        End Set
    End Property

    Public Sub BeginUpdating()
        Listening = True
        Try
            _updateThread = New Thread(AddressOf Me.GetImage)
            _updateThread.Name = "Camera Thread"
            _updateThread.Start()
        Catch ex As Exception
            Try
                If Not _updateThread Is Nothing Then
                    _updateThread.Abort()
                End If
            Catch ex2 As Exception
                'TODO: Exception handling code here.
                Throw
            End Try
        End Try
    End Sub
    Sub GetImage()
        Dim stream As NetworkStream = Nothing
        Dim reader As EndianBinaryReader = Nothing

        While (Listening)
            Try
                If client Is Nothing OrElse Not client.Connected Then
                    client.DisposeIfNotNull()
                    client = New TcpClient()
                    client.ReceiveTimeout = CAMERA_RECEIVE_TIMEOUT
                    client.Connect(RobotAddress, RobotPort)
                End If

                stream = client.GetStream()
                'reader = New BinaryReader(stream)
                reader = New EndianBinaryReader(New BigEndianBitConverter(), stream)
                'State machine state variables
                Dim headerPosition = 0
                Dim mode = 0

                Dim imageData As Byte()
                Dim imageDataLength As UInt32 = 0

                'Dim bytes = reader.ReadBytes(20)

                While (Listening)
                    Select Case mode
                        Case 0 'Header
                            imageData = Nothing
                            imageDataLength = 0
                            Dim b As Byte
                            Try
                                b = reader.ReadByte()
                            Catch ex As SocketException
                                'Debug.Write(String.Format("Image {0} : {1}", ImageCount, ex.Message))
                                mode = 0
                            End Try
                            'Debug.Print(b)
                            Select Case headerPosition
                                Case 0
                                    If b = 1 Then
                                        headerPosition += 1
                                    Else
                                        headerPosition = 0
                                    End If
                                Case Is < 4
                                    If b = 0 Then
                                        headerPosition += 1
                                    Else
                                        headerPosition = 0
                                    End If
                                    If headerPosition = 4 Then
                                        mode += 1
                                        headerPosition = 0
                                    End If
                                Case Else
                                    Throw New InvalidOperationException()
                            End Select
                        Case 1 'Read length
                            Try
                                imageDataLength = reader.ReadUInt32()
                                'Debug.Print(imageDataLength)
                                mode += 1
                            Catch ex As SocketException
                                'Debug.Write(String.Format("Image {0}: {1}", ImageCount, ex.Message))
                                mode = 0
                            End Try
                        Case 2 'Read image
                            Try
                                imageData = reader.ReadBytes(imageDataLength)

                                RaiseNewImageData(imageData)
                            Catch ex As SocketException
                                'Debug.Write(String.Format("Image {0}, length {1}: {2}", ImageCount, imageDataLength, ex.Message))
                            Finally
                                mode = 0
                            End Try
                            'cameraImage = Image.FromStream(reader.BaseStream)
                    End Select
                End While
            Catch ex As Exception
                Debug.WriteLine(String.Format("FATAL ERROR in camera client. {0}", ex.Message))
            Finally
                'Listening = False
                reader.DisposeIfNotNull()
                client.DisposeIfNotNull()
            End Try
        End While

    End Sub


    Public Property RobotAddress() As IPAddress
        Get
            Return _robotAddress
        End Get
        Private Set(ByVal value As IPAddress)
            If _robotAddress Is value Then
                Return
            End If
            _robotAddress = value
        End Set
    End Property
    Public Property RobotPort() As Integer
        Get
            Return _robotPort
        End Get
        Set(ByVal value As Integer)
            If _robotPort = value Then
                Return
            End If
            _robotPort = value
        End Set
    End Property
    Public Property TeamNumber() As Integer
        Get
            Return _teamNumber
        End Get
        Set(ByVal value As Integer)
            If _teamNumber = value Then
                Return
            End If
            _teamNumber = value

            Dim octet2 As Integer
            Dim octet3 As Integer
            octet3 = value Mod 100
            octet2 = (value - octet3) / 100
            Dim ipStr = String.Format("10.{0}.{1}.2", octet2, octet3)
            RobotAddress = IPAddress.Parse(ipStr)
        End Set
    End Property

End Class

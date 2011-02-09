Imports System.Drawing
Imports System.Net.Sockets
Imports System.IO
Imports System.Net
Imports System.Threading
Imports System.ComponentModel

Public Class CameraClient
    Implements INotifyPropertyChanged
    Public Event PropertyChanged(ByVal sender As Object, ByVal e As System.ComponentModel.PropertyChangedEventArgs) Implements System.ComponentModel.INotifyPropertyChanged.PropertyChanged

    Protected Sub RaisePropertyChanged(ByVal prop As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(prop))
    End Sub

    Dim _updateThread As Thread
    Dim _listening As Boolean
    Dim _image As Image
    Dim _imageCount As Integer
    Dim _robotAddress As IPAddress
    Dim _robotPort As Integer = 1180
    Dim _teamNumber As Integer

    Dim client As New TcpClient()

    Public Sub New()
    End Sub

    Public Sub New(ByVal teamNumber As Integer)
        Me.TeamNumber = teamNumber
    End Sub

    Public Property Listening() As Boolean
        Get
            Return _listening
        End Get
        Private Set(ByVal value As Boolean)
            _listening = value
            RaisePropertyChanged("Listening")
        End Set
    End Property

    Public Sub BeginUpdating()
        Listening = True
        Try
            client.Connect(RobotAddress, RobotPort)
            _updateThread = New Thread(AddressOf Me.GetImage)
            _updateThread.Name = "Camera Thread"
            _updateThread.Start()
        Catch ex As Exception
            Try
                If Not _updateThread Is Nothing Then
                    _updateThread.Abort()
                End If
            Catch ex2 As Exception
                Throw
            End Try
        End Try
    End Sub
    Sub GetImage()

        Dim stream = client.GetStream()
        Dim reader = New BinaryReader(stream)

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

                    Dim b = reader.ReadByte()
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
                    'imageDataLength = reader.ReadUInt32()
                    Dim buffer = reader.ReadBytes(4)
                    imageDataLength = buffer(0) * 255 ^ 3 + _
                                        buffer(1) * 255 ^ 2 + _
                                        buffer(2) * 255 + _
                                        buffer(3)
                    'Debug.Print(imageDataLength)
                    mode += 1
                Case 2 'Read image
                    Try
                        imageData = reader.ReadBytes(imageDataLength)
                        Dim memStream = New MemoryStream(imageData, 0, imageDataLength)

                        LastImage = Image.FromStream(memStream)
                        _imageCount += 1
                        memStream.Close()
                    Catch ex As Exception
                        Debug.Write("Image " & ImageCount & ", length " & imageDataLength & ": " & ex.Message)
                    Finally
                        mode = 0
                    End Try
                    'cameraImage = Image.FromStream(reader.BaseStream)
            End Select
        End While
        reader.Close()
        client.Close()
    End Sub

    Public Property LastImage() As Image
        Get
            Return _image
        End Get
        Set(ByVal value As Image)
            If _image Is value Then
                Return
            End If
            _image = value
            RaisePropertyChanged("LastImage")
        End Set
    End Property
    Public Property RobotAddress() As IPAddress
        Get
            Return _robotAddress
        End Get
        Private Set(ByVal value As IPAddress)
            If _robotAddress Is value Then
                Return
            End If
            _robotAddress = value
            RaisePropertyChanged("RobotAddress")
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
            RaisePropertyChanged("RobotPort")
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

            Dim digit1 As Integer
            Dim digit2 As Integer
            digit2 = value Mod 100
            digit1 = (value - digit2) / 100
            Dim ipStr = "10." & digit1 & "." & digit2 & ".2"
            RobotAddress = IPAddress.Parse(ipStr)

            RaisePropertyChanged("TeamNumber")
        End Set
    End Property
    Public Property ImageCount() As Integer
        Get
            Return _imageCount
        End Get
        Private Set(ByVal value As Integer)
            If _imageCount = value Then
                Return
            End If
            _imageCount = value
            RaisePropertyChanged("ImageCount")
        End Set
    End Property
End Class

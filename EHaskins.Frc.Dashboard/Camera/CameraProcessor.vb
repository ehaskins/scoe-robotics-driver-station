Imports System.Windows.Media.Imaging
Imports System.ComponentModel
Imports System.IO
Imports System.Drawing
Imports EHaskins.Frc.Dashboard.Clients

Public Class CameraProcessor
    Implements INotifyPropertyChanged

    Dim _image As BitmapImage
    Dim _drawingImage As Image
    Dim _imageCount As Integer
    Dim _cameraClient As IRawCameraProcessor

    ''' <summary>
    ''' Initializes a new instance of the CameraProcessor class.
    ''' </summary>
    ''' <param name="cameraClient"></param>
    Public Sub New(ByVal cameraClient As IRawCameraProcessor)
        Me.CameraClient = cameraClient
        AddHandler Me.CameraClient.NewImageData, AddressOf Me.NewImageDataHandler
    End Sub

    Public Event PropertyChanged(ByVal sender As Object, ByVal e As System.ComponentModel.PropertyChangedEventArgs) Implements System.ComponentModel.INotifyPropertyChanged.PropertyChanged

    Protected Sub RaisePropertyChanged(ByVal prop As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(prop))
    End Sub

    Public Event NewImage As EventHandler(Of NewImageEventArgs)

    Private Sub RaiseNewImage(ByVal img As BitmapImage, ByVal dImg As Image)
        RaiseEvent NewImage(Me, New NewImageEventArgs(img, dImg))
    End Sub

    Private Sub NewImageDataHandler(ByVal sender As Object, ByVal e As NewImageDataEventArgs)
        ParseData(e.ImageData)
    End Sub

    Private Sub ParseData(ByVal data As Byte())
        Dim memStream = New MemoryStream(data, 0, data.Length())

        Try
            Dim img = New BitmapImage()
            img.BeginInit()
            img.CacheOption = BitmapCacheOption.OnLoad
            img.StreamSource = memStream
            img.Rotation = Rotation
            img.EndInit()
            CurrentImage = img

            'reset stream
            memStream.Position = 0

            Dim dImg = Image.FromStream(memStream)
            DrawingImage = dImg

            RaiseNewImage(img, dImg)
        Catch ex As ArgumentException
            Debug.WriteLine("Invalid image data received")
        End Try
        _imageCount += 1
        memStream.Close()
        memStream.Dispose()

    End Sub


    Public Property CurrentImage() As BitmapImage
        Get
            Return _image
        End Get
        Set(ByVal value As BitmapImage)
            _image = value
            RaisePropertyChanged("CurrentImage")
        End Set
    End Property
    Public Property DrawingImage() As Image
        Get
            Return _drawingImage
        End Get
        Set(ByVal value As Image)
            _drawingImage = value
            RaisePropertyChanged("DrawingImage")
        End Set
    End Property

    Private _rotation As Rotation
    Private Property Rotation() As Rotation
        Get
            Return _rotation
        End Get
        Set(ByVal value As Rotation)
            If value <> _rotation Then
                _rotation = value
                RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs("Rotation"))
            End If
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
    Public Property CameraClient() As IRawCameraProcessor
        Get
            Return _cameraClient
        End Get
        Set(ByVal value As IRawCameraProcessor)
            _cameraClient = value
        End Set
    End Property


End Class

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

Public Class NewImageEventArgs
    Inherits EventArgs

    Dim _image As BitmapImage
    Dim _drawingImage As Image

    Public Sub New()

    End Sub
    ''' <summary>
    ''' Initializes a new instance of the NewImageEventArgs class.
    ''' </summary>
    ''' <param name="image"></param>
    ''' <param name="drawingImage"></param>
    Public Sub New(ByVal image As BitmapImage, ByVal drawingImage As Image)
        _image = image
        _drawingImage = drawingImage
    End Sub
    Public Property DrawingImage() As Image
        Get
            Return _drawingImage
        End Get
        Set(ByVal value As Image)
            _drawingImage = value
        End Set
    End Property
    Public Property Image() As BitmapImage
        Get
            Return _image
        End Get
        Set(ByVal value As BitmapImage)
            _image = value
        End Set
    End Property
End Class

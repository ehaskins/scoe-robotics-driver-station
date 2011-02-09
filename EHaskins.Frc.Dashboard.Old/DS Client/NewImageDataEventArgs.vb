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



Public Class NewImageDataEventArgs
    Inherits EventArgs
    Public Sub New(ByVal data As Byte())
        ImageData = data
    End Sub

    Private _imageData As Byte()
    Public Property ImageData() As Byte()
        Get
            Return _imageData
        End Get
        Set(ByVal Value As Byte())
            _imageData = Value
        End Set
    End Property
End Class

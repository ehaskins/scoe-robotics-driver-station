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
Public Interface ICameraClient
    Event NewImageData As EventHandler(Of NewImageDataEventArgs)
    'Property ImageCount() As Integer
    'Property DrawingImage() As Image
    'Property CurrentImage() As BitmapImage
    'Event NewImage As EventHandler(Of NewImageEventArgs)
End Interface

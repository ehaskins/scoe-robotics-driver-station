Imports System.Net.Sockets
Imports System.Threading
Imports System.Net
Imports System.IO
Imports System.ComponentModel

Public Interface IDashboardClient
    Property Listening() As Boolean
    Sub BeginGetDataAsync()
    Sub GetDataAsync(ByVal result As IAsyncResult)
    Sub EndGetDataAsync()
    Sub GetData()
    Sub BeginUpdating()
    Sub ParseBytes(ByVal data As Byte())
End Interface

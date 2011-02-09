Imports System.Net.Sockets
Imports System.Threading
Imports System.Net
Imports System.IO
Imports System.ComponentModel


Public Class NewDashboardDataEventArgs
    Inherits EventArgs

    Dim data As Byte()
    Public Sub New(ByVal data As Byte())
        DashboardData = data
    End Sub
    Public Sub New()

    End Sub

    Public Property DashboardData() As Byte()
        Get
            Return data
        End Get
        Set(ByVal value As Byte())
            data = value
        End Set
    End Property


End Class

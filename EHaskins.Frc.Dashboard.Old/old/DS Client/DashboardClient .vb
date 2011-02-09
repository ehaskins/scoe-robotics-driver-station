Public Class DashboardClient
    Inherits DashboardClient(Of DefaultUserDataProcessor)

    ''' <summary>
    ''' Initializes a new instance of the DashboardClient class.
    ''' </summary>
    Public Sub New()
        MyBase.New(New DefaultUserDataProcessor)
    End Sub
End Class
Public CLass RobotStatus
    ''' <summary>
    ''' Initializes a new instance of the RobotStatus structure.
    ''' </summary>
    ''' <param name="data"></param>
    Public Sub New(ByVal data As Byte)
        Me.Data = data
    End Sub
    Public Data As Byte
End Class
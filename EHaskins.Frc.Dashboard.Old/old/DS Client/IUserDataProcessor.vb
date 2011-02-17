Public Interface IUserDataProcessor
    Inherits IDashboardDataProcessor

    ReadOnly Property ProcessorOriginId() As Integer
    ReadOnly Property ProcessorId() As Integer
End Interface

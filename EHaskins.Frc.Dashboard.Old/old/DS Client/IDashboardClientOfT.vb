Public Interface IDashboardClient(Of UserDataProcessor As IDashboardDataProcessor)
    Inherits IDashboardClient
    Property UserData() As UserDataProcessor
End Interface

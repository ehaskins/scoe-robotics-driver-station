Public Interface IDashboardDataProcessor
    Function Parse(ByVal data As Byte()) As Byte()
    Function ShallowParse(ByVal data As Byte()) As Byte()
End Interface

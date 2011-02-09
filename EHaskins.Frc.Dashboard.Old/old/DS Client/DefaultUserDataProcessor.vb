Imports System.IO
Imports System.ComponentModel
Imports System.Collections.ObjectModel
Imports System.Runtime.CompilerServices


Public Class DefaultUserDataProcessor
    Inherits NonUserDataProcessor
    Implements IUserDataProcessor

    Public Sub New()
        MyBase.new()
        SetId(0, 0)
        _printedStrings = New ObservableCollection(Of String)
        _errorStrings = New ObservableCollection(Of String)
    End Sub

    Private _processorId As Integer = 0
    Private _processorOriginId As Integer = 0
    Dim _sequenceNumber As Integer
    Dim _printedStrings As ObservableCollection(Of String)
    Dim _errorStrings As ObservableCollection(Of String)
    Dim _userData As Byte()

    Public Overrides Function Parse(ByVal data() As Byte) As Byte()
        Dim myData = MyBase.Parse(data)
        Return MyClass.ShallowParse(myData)

    End Function

    Public Overrides Function ShallowParse(ByVal data As Byte()) As Byte()
        Dim reader As New BinaryReader(New MemoryStream(data))

        SequenceNumber = reader.ReadByte()

        Dim printedString = New String(Text.Encoding.ASCII.GetChars(reader.ReadBytes(reader.EReadUInt32())))
        Dim strings = printedString.Split(vbNewLine)
        For Each s In strings
            If Not String.IsNullOrEmpty(s) Then
                Me.PrintedStrings.Add(s)
            End If
        Next

        Dim errorString = New String(Text.Encoding.ASCII.GetChars(reader.ReadBytes(reader.EReadUInt32())))
        strings = errorString.Split(vbNewLine)
        For Each s In strings
            If Not String.IsNullOrEmpty(s) Then
                Me.ErrorStrings.Add(s)
            End If
        Next

        Dim dataSize = reader.EReadUInt32()
        Dim userData As Byte() = reader.ReadBytes(dataSize)

        MyClass.UserData = userData
        Return userData
    End Function

    Public Property ErrorStrings() As ObservableCollection(Of String)
        Get
            Return _errorStrings
        End Get
        Set(ByVal value As ObservableCollection(Of String))
            If _errorStrings Is value Then
                Return
            End If
            _errorStrings = value
            RaisePropertyChanged("ErrorStrings")
        End Set
    End Property
    Public Property PrintedStrings() As ObservableCollection(Of String)
        Get
            Return _printedStrings
        End Get
        Set(ByVal value As ObservableCollection(Of String))
            If _printedStrings Is value Then
                Return
            End If
            _printedStrings = value
            RaisePropertyChanged("PrintedStrings")
        End Set
    End Property
    Public Property SequenceNumber() As Integer
        Get
            Return _sequenceNumber
        End Get
        Set(ByVal value As Integer)
            If _sequenceNumber = value Then
                Return
            End If
            _sequenceNumber = value
            RaisePropertyChanged("SequenceNumber")
        End Set
    End Property
    Public Overridable Property UserData() As Byte()
        Get
            Return _userData
        End Get
        Set(ByVal value As Byte())
            _userData = value
            RaisePropertyChanged("UserData")
        End Set
    End Property

    Public ReadOnly Property ProcessorId() As Integer Implements IUserDataProcessor.ProcessorId
        Get
            Return _processorId
        End Get
    End Property
    Public ReadOnly Property ProcessorOriginId() As Integer Implements IUserDataProcessor.ProcessorOriginId
        Get
            Return _processorOriginId
        End Get
    End Property

    Protected Sub SetId(ByVal origin As Integer, ByVal Id As Integer)
        _processorId = Id
        _processorOriginId = origin
    End Sub
End Class

Public Class FilterVM(Of T)
    Inherits BindableObject
    Implements IFilterVM
    Implements IEqualityComparer(Of T)

    Private ReadOnly keySelector As Func(Of T, Integer)
    Private ReadOnly defaultText As Func(Of T, String)
    Private ReadOnly acceptingText As Func(Of T, String())

    Public Sub New(name As String, keySelector As Func(Of T, Integer), defaultText As Func(Of T, String), Optional acceptingText As Func(Of T, String()) = Nothing)
        Me.Name = name
        Me.keySelector = keySelector
        Me.defaultText = defaultText
        Me.acceptingText = acceptingText
    End Sub

    Public Function Hits(value As T) As Boolean
        If Not IsEnabled Then Return True
        If defaultText(value).Contains(SelectedText) Then Return True
        If acceptingText Is Nothing Then Return False
        For Each str In acceptingText(value)
            If str.Contains(SelectedText) Then Return True
        Next
        Return False
    End Function

    Public ReadOnly Property Name As String Implements IFilterVM.Name

    Private _isEnabled As Boolean
    Public Property IsEnabled As Boolean Implements IFilterVM.IsEnabled
        Get
            Return _isEnabled
        End Get
        Set(value As Boolean)
            [Set](_isEnabled, value)
        End Set
    End Property
    Public Property SelectedText As String Implements IFilterVM.SelectedText

    Private _candidates As IBindableCollection(Of String)
    Public ReadOnly Property Candidates As IBindableCollection(Of String) Implements IFilterVM.Candidates
        Get
            Return _candidates
        End Get
    End Property
    Public Sub UpdateCandidates(values As IEnumerable(Of T))
        _candidates = values.Distinct(Me).OrderBy(keySelector).Select(defaultText).Distinct().ToBindable()
        NotifyPropertyChanged(NameOf(Candidates))
    End Sub

    Private Overloads Function Equals(x As T, y As T) As Boolean Implements IEqualityComparer(Of T).Equals
        Return keySelector(x) = keySelector(y)
    End Function

    Private Overloads Function GetHashCode(obj As T) As Integer Implements IEqualityComparer(Of T).GetHashCode
        Return keySelector(obj)
    End Function
End Class

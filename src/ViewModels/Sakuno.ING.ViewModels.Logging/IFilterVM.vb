Public Interface IFilterVM
    Inherits IBindable

    ReadOnly Property Name As String
    Property IsEnabled As Boolean
    Property SelectedText As String
    ReadOnly Property Candidates As IBindableCollection(Of String)
End Interface

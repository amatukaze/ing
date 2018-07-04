Imports Sakuno.ING.Game
Imports Sakuno.ING.Game.Logger

Public MustInherit Class LogsVM(Of T As {Class, ITimedEntity})
    Protected MustOverride Function CreateFilters() As FilterVM(Of T)()
    Protected MustOverride Function GetEntities() As IReadOnlyCollection(Of T)

    Private ReadOnly _filters As IBindableCollection(Of FilterVM(Of T))
    Public ReadOnly Property Filters As IBindableCollection(Of IFilterVM)
        Get
            Return _filters
        End Get
    End Property

    Private ReadOnly _snapshot As New BindableSnapshotCollection(Of T)
    Public ReadOnly Property Entities As IBindableCollection(Of T)
        Get
            Return _snapshot
        End Get
    End Property

    Public Sub New()
        _filters = CreateFilters().ToBindable()
    End Sub

    Public Sub Refresh()
        Dim source = GetEntities().AsEnumerable()
        For Each filter In _filters
            filter.UpdateCandidates(source)
            source = source.Where(AddressOf filter.Hits)
        Next
        _snapshot.Query = source
    End Sub
End Class

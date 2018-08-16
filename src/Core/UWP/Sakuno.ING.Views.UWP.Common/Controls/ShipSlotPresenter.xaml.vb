Imports Sakuno.ING.Game.Models

Namespace Controls
    Public NotInheritable Class ShipSlotPresenter
        Inherits UserControl

        Public Sub New()
            InitializeComponent()
        End Sub

        Private _slot As Slot
        Public Property Slot As Slot
            Get
                Return _slot
            End Get
            Set(value As Slot)
                _slot = value
                Bindings.Update()
            End Set
        End Property

    End Class
End Namespace
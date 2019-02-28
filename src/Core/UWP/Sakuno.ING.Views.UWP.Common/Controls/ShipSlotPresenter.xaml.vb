Imports Sakuno.ING.Game.Models

Namespace Controls
    Public NotInheritable Class ShipSlotPresenter
        Inherits UserControl

        Public Sub New()
            InitializeComponent()
        End Sub

        Private _slot As HomeportSlot
        Public Property Slot As HomeportSlot
            Get
                Return _slot
            End Get
            Set(value As HomeportSlot)
                _slot = value
                Bindings.Update()
            End Set
        End Property

    End Class
End Namespace

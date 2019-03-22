Imports Sakuno.ING.Game.Models
Imports Windows.UI

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

        Public Shared Function SelectImprovementText(level As Integer) As String
            Select Case level
                Case 0
                    Return Nothing
                Case 10
                    Return "max"
                Case Else
                    Return "★" & level
            End Select
        End Function

        Public Shared Function SelectAircraftColor(isPlane As Boolean, aircraft As ClampedValue) As Color
            If Not isPlane Then
                Return Colors.Gray 'Not plane
            ElseIf aircraft.IsMaximum Then
                Return Colors.SpringGreen 'Full
            ElseIf aircraft.Current = 0 Then
                Return Colors.Red 'Empty
            Else
                Return Colors.YellowGreen 'Half
            End If
        End Function
    End Class
End Namespace

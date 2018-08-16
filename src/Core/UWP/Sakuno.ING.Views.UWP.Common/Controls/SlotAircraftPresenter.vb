Imports Sakuno.ING.Game.Models

Namespace Controls
    <TemplatePart(Name:="PART_Text", Type:=GetType(TextBlock))>
    <TemplateVisualState(GroupName:="State", Name:="NotPlane")>
    <TemplateVisualState(GroupName:="State", Name:="Full")>
    <TemplateVisualState(GroupName:="State", Name:="Empty")>
    <TemplateVisualState(GroupName:="State", Name:="Half")>
    Public NotInheritable Class SlotAircraftPresenter
        Inherits Control

        Private _isPlane As Boolean
        Public Property IsPlane As Boolean
            Get
                Return _isPlane
            End Get
            Set(value As Boolean)
                _isPlane = value
                CheckVisualState()
            End Set
        End Property

        Private _aircraft As ClampedValue
        Public Property Aircraft As ClampedValue
            Get
                Return _aircraft
            End Get
            Set(value As ClampedValue)
                _aircraft = value
                UpdateCount()
            End Set
        End Property

        Public Sub New()
            DefaultStyleKey = GetType(SlotAircraftPresenter)
        End Sub

        Private Sub CheckVisualState()
            If Not IsPlane Then
                VisualStateManager.GoToState(Me, "NotPlane", True)
            ElseIf Aircraft.Current = Aircraft.Max Then
                VisualStateManager.GoToState(Me, "Full", True)
            ElseIf Aircraft.Current = 0 Then
                VisualStateManager.GoToState(Me, "Empty", True)
            Else
                VisualStateManager.GoToState(Me, "Half", True)
            End If
        End Sub

        Private Sub UpdateCount()
            If PART_Text IsNot Nothing Then PART_Text.Text = Aircraft.Current.ToString()
            CheckVisualState()
        End Sub

        Private PART_Text As TextBlock
        Protected Overrides Sub OnApplyTemplate()
            MyBase.OnApplyTemplate()
            PART_Text = TryCast(GetTemplateChild("PART_Text"), TextBlock)
            UpdateCount()
        End Sub

    End Class
End Namespace
Namespace Controls
    <TemplateVisualState(GroupName:="Status", Name:="NotImproved")>
    <TemplateVisualState(GroupName:="Status", Name:="Improved")>
    <TemplateVisualState(GroupName:="Status", Name:="MaxImproved")>
    Public NotInheritable Class EquipmentImprovementPresenter
        Inherits Control

        Public Shared ReadOnly LevelProperty As DependencyProperty =
            DependencyProperty.Register(NameOf(Level), GetType(Integer), GetType(EquipmentImprovementPresenter),
                                        New PropertyMetadata(0, Sub(d, e) CType(d, EquipmentImprovementPresenter).CheckVisualState()))
        Public Property Level As Integer
            Get
                Return CType(GetValue(LevelProperty), Integer)
            End Get
            Set(value As Integer)
                SetValue(LevelProperty, value)
            End Set
        End Property

        Public Sub New()
            DefaultStyleKey = GetType(EquipmentImprovementPresenter)
            CheckVisualState()
        End Sub

        Private Sub CheckVisualState()
            Select Case Level
                Case 0
                    VisualStateManager.GoToState(Me, "NotImproved", True)
                Case 10
                    VisualStateManager.GoToState(Me, "MaxImproved", True)
                Case Else
                    VisualStateManager.GoToState(Me, "Improved", True)
            End Select
        End Sub

    End Class
End Namespace
Namespace Controls
    Public NotInheritable Class MoraleIndicator
        Inherits Control

        Public Sub New()
            Me.DefaultStyleKey = GetType(MoraleIndicator)
        End Sub

        Public ReadOnly MoraleProperty As DependencyProperty =
            DependencyProperty.Register(NameOf(Morale), GetType(Integer), GetType(MoraleIndicator),
                                        New PropertyMetadata(0, Sub(d, e) DirectCast(d, MoraleIndicator).CheckVisualState()))
        Public Property Morale As Integer
            Get
                Return DirectCast(GetValue(MoraleProperty), Integer)
            End Get
            Set(value As Integer)
                SetValue(MoraleProperty, value)
            End Set
        End Property

        Protected Overrides Sub OnApplyTemplate()
            MyBase.OnApplyTemplate()
            CheckVisualState()
        End Sub

        Private Sub CheckVisualState()
            Select Case Morale
                Case > 85
                    VisualStateManager.GoToState(Me, "Full", True)
                Case > 49
                    VisualStateManager.GoToState(Me, "High", True)
                Case > 39
                    VisualStateManager.GoToState(Me, "Normal", True)
                Case > 29
                    VisualStateManager.GoToState(Me, "Slight", True)
                Case > 19
                    VisualStateManager.GoToState(Me, "Moderate", True)
                Case Else
                    VisualStateManager.GoToState(Me, "Serious", True)
            End Select
        End Sub
    End Class
End Namespace

Imports Sakuno.ING.Game.Models.Knowledge

Namespace Controls
    <TemplatePart(Name:=UseItemPresenter.Name_PART_Difference, Type:=GetType(TextBlock))>
    Public NotInheritable Class UseItemPresenter
        Inherits Control

        Friend Const Name_PART_Difference = "PART_Difference"

        Public Shared ReadOnly IdProperty As DependencyProperty =
            DependencyProperty.Register(NameOf(Id), GetType(KnownUseItem), GetType(UseItemPresenter), New PropertyMetadata(New KnownUseItem))
        Public Property Id As KnownUseItem
            Get
                Return CType(GetValue(IdProperty), KnownUseItem)
            End Get
            Set(value As KnownUseItem)
                SetValue(IdProperty, value)
            End Set
        End Property

        Public Shared ReadOnly AmountProperty As DependencyProperty =
            DependencyProperty.Register(NameOf(Amount), GetType(Integer), GetType(UseItemPresenter),
                                        New PropertyMetadata(0, Sub(d, e) CType(d, UseItemPresenter).Update(CInt(e.OldValue), CInt(e.NewValue))))
        Public Property Amount As Integer
            Get
                Return CType(GetValue(AmountProperty), Integer)
            End Get
            Set(value As Integer)
                SetValue(AmountProperty, value)
            End Set
        End Property

        Private _updateNestingCount As Integer

        Private _difference As Integer
        Private _differenceText As TextBlock

        Public Sub New()
            DefaultStyleKey = GetType(UseItemPresenter)
        End Sub

        Protected Overrides Sub OnApplyTemplate()
            MyBase.OnApplyTemplate()
            _differenceText = TryCast(GetTemplateChild(Name_PART_Difference), TextBlock)
        End Sub

        Private Sub Update(oldValue As Integer, newValue As Integer)
            _difference += newValue - oldValue
            _differenceText.Text = _difference.ToString("+0;-0;0")

            UpdateVisualState()
        End Sub

        Private Async Sub UpdateVisualState()
            Const ThrottleTime = 3000

            VisualStateManager.GoToState(Me, "ValueChanged", True)

            _updateNestingCount = _updateNestingCount + 1
            Await Task.Delay(ThrottleTime)
            _updateNestingCount = _updateNestingCount - 1

            If _updateNestingCount > 0 Then
                Return
            End If

            VisualStateManager.GoToState(Me, "Normal", True)

            _difference = 0
            _differenceText.Text = String.Empty
        End Sub

    End Class
End Namespace

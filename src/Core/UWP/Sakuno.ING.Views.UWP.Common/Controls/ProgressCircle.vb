Imports Windows.UI.Xaml.Shapes

Namespace Controls
    <TemplatePart(Name:=ProgressCircle.Name_PART_Progress, Type:=GetType(Ellipse))>
    <TemplatePart(Name:=ProgressCircle.Name_PART_Back, Type:=GetType(Ellipse))>
    Public Class ProgressCircle
        Inherits RangeBase
        Friend Const Name_PART_Progress As String = "PART_Progress"
        Friend Const Name_PART_Back As String = "PART_Back"

        Public Sub New()
            DefaultStyleKey = GetType(ProgressCircle)
        End Sub

        Protected Overrides Function MeasureOverride(availableSize As Size) As Size
            Return availableSize
        End Function

        Protected Overrides Function ArrangeOverride(finalSize As Size) As Size
            Dim min = Math.Min(finalSize.Height, finalSize.Width)
            diameter = min
            UpdateBaseline()
            Return MyBase.ArrangeOverride(New Size(min, min))
        End Function

        Private circle, back As Ellipse
        Protected Overrides Sub OnApplyTemplate()
            MyBase.OnApplyTemplate()
            circle = TryCast(GetTemplateChild(Name_PART_Progress), Ellipse)
            back = TryCast(GetTemplateChild(Name_PART_Back), Ellipse)
            UpdateBaseline()
        End Sub

        Public Shared ReadOnly StrokeThicknessProperty As DependencyProperty =
            DependencyProperty.Register(NameOf(StrokeThickness), GetType(Double), GetType(ProgressCircle),
                New PropertyMetadata(4.0))
        Public Property StrokeThickness As Double
            Get
                Return DirectCast(GetValue(StrokeThicknessProperty), Double)
            End Get
            Set(value As Double)
                SetValue(StrokeThicknessProperty, value)
            End Set
        End Property

        Private diameter, roundRate As Double
        Private Sub UpdateBaseline()
            roundRate = (diameter - StrokeThickness) * Math.PI / StrokeThickness
            If circle IsNot Nothing Then
                circle.StrokeDashArray = New DoubleCollection From {0, roundRate * 2, roundRate * 2, 0}
                UpdateOffset()
            End If
        End Sub

        Private Sub UpdateOffset()
            Dim rate = (Value - Minimum) / (Maximum - Minimum)
            If circle IsNot Nothing Then
                circle.StrokeDashOffset = -rate * roundRate
            End If
        End Sub

        Protected Overrides Sub OnMaximumChanged(oldMaximum As Double, newMaximum As Double)
            UpdateOffset()
        End Sub
        Protected Overrides Sub OnMinimumChanged(oldMaximum As Double, newMaximum As Double)
            UpdateOffset()
        End Sub
        Protected Overrides Sub OnValueChanged(oldMaximum As Double, newMaximum As Double)
            UpdateOffset()
        End Sub
    End Class
End Namespace

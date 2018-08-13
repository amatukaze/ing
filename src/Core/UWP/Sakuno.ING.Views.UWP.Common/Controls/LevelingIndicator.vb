Imports Sakuno.ING.Game.Models
Imports Windows.UI

Namespace Controls
    <TemplatePart(Name:=LevelingIndicator.Name_PART_Range, Type:=GetType(RangeBase))>
    <TemplatePart(Name:=LevelingIndicator.Name_PART_Level, Type:=GetType(TextBlock))>
    Public Class LevelingIndicator
        Inherits Control
        Friend Const Name_PART_Range = "PART_Range"
        Friend Const Name_PART_Level = "PART_Level"

        Public Sub New()
            DefaultStyleKey = GetType(LevelingIndicator)
        End Sub

        Private _level As Leveling
        Public Property Level As Leveling
            Get
                Return _level
            End Get
            Set(value As Leveling)
                _level = value
                Update()
            End Set
        End Property

        Public Shared ReadOnly TextForegroundProperty As DependencyProperty =
            DependencyProperty.Register(NameOf(TextForeground), GetType(Brush), GetType(LevelingIndicator),
                New PropertyMetadata(New SolidColorBrush(Colors.Transparent)))
        Public Property TextForeground As Brush
            Get
                Return DirectCast(GetValue(TextForegroundProperty), Brush)
            End Get
            Set(value As Brush)
                SetValue(TextForegroundProperty, value)
            End Set
        End Property

        Private PART_Range As RangeBase
        Private PART_Level As TextBlock
        Protected Overrides Sub OnApplyTemplate()
            MyBase.OnApplyTemplate()
            PART_Range = TryCast(GetTemplateChild(Name_PART_Range), RangeBase)
            PART_Level = TryCast(GetTemplateChild(Name_PART_Level), TextBlock)
            Update()
        End Sub

        Private Sub Update()
            Dim l = Level

            If PART_Range IsNot Nothing Then
                If l.CurrentLevelExperience >= l.NextLevelExperience Then
                    PART_Range.Minimum = 0
                    PART_Range.Maximum = 1
                    PART_Range.Value = 1
                Else
                    PART_Range.Minimum = l.CurrentLevelExperience
                    PART_Range.Maximum = l.NextLevelExperience
                    PART_Range.Value = l.Experience
                End If
            End If

            If PART_Level IsNot Nothing Then
                PART_Level.Text = l.Level.ToString()
            End If
        End Sub
    End Class
End Namespace
Imports Sakuno.ING.Game.Models

Namespace Controls
    <TemplatePart(Name:="PART_Main", Type:=GetType(RangeBase))>
    <TemplatePart(Name:="PART_Addition", Type:=GetType(RangeBase))>
    Public NotInheritable Class HPIndicator
        Inherits Control

        Public Sub New()
            DefaultStyleKey = GetType(HPIndicator)
        End Sub

        Private _current, _max As Integer
        Public Property HP As ShipHP
            Get
                Return (_current, _max)
            End Get
            Set(value As ShipHP)
                If _max <> value.Max Then
                    _max = value.Max
                    UpdateMax()
                End If
                If _current <> value.Current Then
                    _current = value.Current
                    UpdateCurrent()
                End If
            End Set
        End Property

        Private _additional As Integer?
        Public Property Additional As Integer?
            Get
                Return _additional
            End Get
            Set(value As Integer?)
                If _additional <> value Then
                    _additional = value
                    UpdateAdditional()
                End If
            End Set
        End Property

        Private _additionalBrush As Brush
        Public Property AdditionalBrush As Brush
            Get
                Return _additionalBrush
            End Get
            Set(value As Brush)
                _additionalBrush = value
                If PART_Addition IsNot Nothing Then PART_Addition.Foreground = value
            End Set
        End Property

        Private PART_Main, PART_Addition As RangeBase

        Protected Overrides Sub OnApplyTemplate()
            MyBase.OnApplyTemplate()
            PART_Main = TryCast(GetTemplateChild(NameOf(PART_Main)), RangeBase)
            PART_Addition = TryCast(GetTemplateChild(NameOf(PART_Addition)), RangeBase)
            If PART_Main IsNot Nothing Then PART_Main.Minimum = 0
            If PART_Addition IsNot Nothing Then
                PART_Addition.Minimum = 0
                PART_Addition.Foreground = AdditionalBrush
            End If
            UpdateMax()
            UpdateCurrent()
            UpdateAdditional()
        End Sub

        Private Sub UpdateMax()
            If PART_Main IsNot Nothing Then PART_Main.Maximum = _max
            If PART_Addition IsNot Nothing Then PART_Addition.Maximum = _max
        End Sub

        Private Sub UpdateCurrent()
            If PART_Main IsNot Nothing Then PART_Main.Value = _current
            VisualStateManager.GoToState(Me, HP.DamageState.ToString(), True)
        End Sub

        Private Sub UpdateAdditional()
            If PART_Addition IsNot Nothing Then
                PART_Addition.Value = If(_additional, 0)
                PART_Addition.Visibility = If(_additional Is Nothing, Visibility.Collapsed, Visibility.Visible)
            End If
        End Sub
    End Class
End Namespace

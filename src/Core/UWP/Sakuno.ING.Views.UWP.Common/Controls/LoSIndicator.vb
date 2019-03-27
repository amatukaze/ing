Imports Sakuno.ING.Game.Models
Imports Sakuno.ING.ViewModels.Homeport

Namespace Controls
    Public NotInheritable Class LoSIndicator
        Inherits Control

        Public Sub New()
            DefaultStyleKey = GetType(LoSIndicator)
        End Sub

        Public ReadOnly Property ViewModel As New FleetLoSVM

        Public Property Effective As LineOfSight
            Get
                Return ViewModel.Effective
            End Get
            Set(value As LineOfSight)
                ViewModel.Effective = value
            End Set
        End Property

        Public Property Simple As Integer
            Get
                Return ViewModel.Simple
            End Get
            Set(value As Integer)
                ViewModel.Simple = value
            End Set
        End Property
    End Class
End Namespace

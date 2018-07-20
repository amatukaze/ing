Imports Sakuno.ING.Composition
Imports Sakuno.ING.Game
Imports Sakuno.ING.Game.Models.MasterData
Imports Sakuno.ING.Settings

Public Module Helpers
    Public Function IsNotNull(obj As Object) As Boolean
        Return obj IsNot Nothing
    End Function

    Public Function [Not](value As Boolean) As Boolean
        Return Not value
    End Function

    Public Function Format(formatString As String, arg0 As Object) As String
        Return String.Format(formatString, arg0)
    End Function

    Public Function Format(formatString As String, arg0 As Object, arg1 As Object) As String
        Return String.Format(formatString, arg0, arg1)
    End Function

    Private ReadOnly lazyLocaleSetting As New Lazy(Of LocaleSetting)(Function() Compositor.Static(Of LocaleSetting)())

    Public Function SelectName(name As TextTranslationGroup) As String
        Return If(lazyLocaleSetting.Value.TranslateContent.InitialValue, name.Translation, name.Origin)
    End Function

    Public Function SelectShipName(name As ShipName) As String
        Return SelectName(name)
    End Function
End Module

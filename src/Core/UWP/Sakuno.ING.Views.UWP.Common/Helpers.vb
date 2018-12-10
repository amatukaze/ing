Imports Sakuno.ING.Composition
Imports Sakuno.ING.Game
Imports Sakuno.ING.Game.Models
Imports Sakuno.ING.Game.Models.MasterData
Imports Sakuno.ING.Localization
Imports Sakuno.ING.Settings

Public Module Helpers
    Public Function IsNotNull(obj As Object) As Boolean
        Return obj IsNot Nothing
    End Function

    Public Function [Not](value As Boolean) As Boolean
        Return Not value
    End Function

    Private ReadOnly lazyLocaleSetting As New Lazy(Of LocaleSetting)(Function() Compositor.Static(Of LocaleSetting)())

    Public Function SelectName(name As TextTranslationGroup) As String
        Return If(lazyLocaleSetting.Value.TranslateContent.InitialValue, name.Translation, name.Origin)
    End Function

    Public Function SelectShipName(name As ShipName) As String
        Return SelectName(name)
    End Function

    Sub New()
        Dim localization = Compositor.Static(Of ILocalizationService)
        admiralRankStrings = Enumerable.Range(1, 10).Select(Function(i) localization.GetLocalized("GameModel", "AdmiralRank_" & i)).ToArray()
        fireRangeTexts = Enumerable.Range(0, 5).Select(Function(i) localization.GetLocalized("GameModel", "FireRange_" & i)).ToArray()
        shipSpeedTexts = Enumerable.Range(0, 5).Select(Function(i) localization.GetLocalized("GameModel", "ShipSpeed_" & (i * 5))).ToArray()
        dockEmpty = localization.GetLocalized("GameModel", "Dock_Empty")
        dockLocked = localization.GetLocalized("GameModel", "Dock_Locked")
    End Sub

    Private ReadOnly admiralRankStrings As String()
    Public Function FormatAdmiralRank(admiral As AdmiralRank) As String
        Dim i = admiral - 1
        If i >= 0 AndAlso i < 10 Then
            Return admiralRankStrings(i)
        Else
            Return Nothing
        End If
    End Function

    Private ReadOnly fireRangeTexts As String()
    Public Function FormatFireRange(range As FireRange) As String
        Select Case range
            Case FireRange.None
                Return fireRangeTexts(0)
            Case FireRange.Short
                Return fireRangeTexts(1)
            Case FireRange.Medium
                Return fireRangeTexts(2)
            Case FireRange.Long
                Return fireRangeTexts(3)
            Case FireRange.VeryLong
                Return fireRangeTexts(4)
            Case Else
                Return Nothing
        End Select
    End Function

    Private ReadOnly shipSpeedTexts As String()
    Public Function FormatShipSpeed(speed As ShipSpeed) As String
        Select Case speed
            Case ShipSpeed.None
                Return shipSpeedTexts(0)
            Case ShipSpeed.Slow
                Return shipSpeedTexts(1)
            Case ShipSpeed.Fast
                Return shipSpeedTexts(2)
            Case ShipSpeed.FastPlus
                Return shipSpeedTexts(3)
            Case ShipSpeed.UltraFast
                Return shipSpeedTexts(4)
            Case Else
                Return Nothing
        End Select
    End Function

    Private ReadOnly dockEmpty, dockLocked As String
    Public Function FormatBuildingDockState(state As BuildingDockState) As String
        Select Case state
            Case BuildingDockState.Empty
                Return dockEmpty
            Case BuildingDockState.Locked
                Return dockLocked
            Case Else
                Return Nothing
        End Select
    End Function
    Public Function FormatRepairingDockState(state As RepairingDockState) As String
        Select Case state
            Case RepairingDockState.Empty
                Return dockEmpty
            Case RepairingDockState.Locked
                Return dockLocked
            Case Else
                Return Nothing
        End Select
    End Function

    Public Function FleetStateEquals(left As FleetState, right As FleetState) As Boolean
        Return left = right
    End Function
End Module

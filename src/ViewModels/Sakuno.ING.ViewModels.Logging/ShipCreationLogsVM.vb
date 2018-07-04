Imports Sakuno.ING.Composition
Imports Sakuno.ING.Game.Logger
Imports Sakuno.ING.Game.Logger.Models
Imports Sakuno.ING.Game.Models

<Export(GetType(ShipCreationLogsVM), SingleInstance:=False)>
Public Class ShipCreationLogsVM
    Inherits LogsVM(Of ShipCreationModel)

    Private ReadOnly logger As Logger
    Private ReadOnly masterData As MasterDataRoot

    Public Sub New(logger As Logger, navalBase As NavalBase)
        Me.logger = logger
        masterData = navalBase.MasterData
        Refresh()
    End Sub

    Protected Overrides Function CreateFilters() As FilterVM(Of ShipCreationModel)()
        Return {
            New FilterVM(Of ShipCreationModel)("Is LSC", Function(x) If(x.IsLSC, 0, 1), Function(x) x.IsLSC.ToString()),
            New FilterVM(Of ShipCreationModel)("Ship built", Function(x) x.ShipBuilt.Id, Function(x) x.ShipBuilt.Name),
            New FilterVM(Of ShipCreationModel)("Secretary", Function(x) x.Secretary.Id, Function(x) x.Secretary.Name)
        }
    End Function

    Protected Overrides Function GetEntities() As IReadOnlyCollection(Of ShipCreationModel)
        If Not logger.PlayerLoaded Then Return Array.Empty(Of ShipCreationModel)
        Using context = logger.CreateContext()
            Return (From e In context.ShipCreationTable.AsEnumerable()
                    Select New ShipCreationModel(masterData.ShipInfos, e)).ToList()
        End Using
    End Function
End Class

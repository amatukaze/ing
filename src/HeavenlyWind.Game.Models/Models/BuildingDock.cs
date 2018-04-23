namespace Sakuno.KanColle.Amatsukaze.Game.Models
{
    partial class BuildingDock
    {
        partial void UpdateCore(IRawBuildingDock raw)
        {
            BuiltShip = shipInfos[raw.BuiltShipId];
        }
    }
}

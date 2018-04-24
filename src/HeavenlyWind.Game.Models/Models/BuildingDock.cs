namespace Sakuno.KanColle.Amatsukaze.Game.Models
{
    partial class BuildingDock
    {
        partial void UpdateCore(IRawBuildingDock raw)
        {
            BuiltShip = shipInfoTable[raw.BuiltShipId];
        }
    }
}

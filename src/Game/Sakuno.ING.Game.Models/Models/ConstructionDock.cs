namespace Sakuno.ING.Game.Models
{
    public partial class ConstructionDock
    {
        partial void UpdateCore(RawConstructionDock raw)
        {
            BuiltShip = _owner.MasterData.ShipInfos[raw.BuiltShipId];
        }

        internal void InstantBuild()
        {
            State = ConstructionDockState.Completed;
            CompletionTime = null;
        }
    }
}

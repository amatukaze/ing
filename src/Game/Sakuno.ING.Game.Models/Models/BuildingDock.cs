using System;

namespace Sakuno.ING.Game.Models
{
    public partial class BuildingDock
    {
        partial void UpdateCore(IRawBuildingDock raw, DateTimeOffset timeStamp)
        {
            BuiltShip = owner.MasterData.ShipInfos[raw.BuiltShipId];
            UpdateTimer(timeStamp);
        }

        internal void UpdateTimer(DateTimeOffset timeStamp)
        {
            if (BuiltShip == null)
                TimeRemaining = default;
            else
                TimeRemaining = CompletionTime - timeStamp;
        }
    }
}

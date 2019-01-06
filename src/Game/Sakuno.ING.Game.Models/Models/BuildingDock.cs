using System;

namespace Sakuno.ING.Game.Models
{
    public partial class BuildingDock
    {
        partial void UpdateCore(RawBuildingDock raw, DateTimeOffset timeStamp)
        {
            BuiltShip = owner.MasterData.ShipInfos[raw.BuiltShipId];
            UpdateTimer(timeStamp);
        }

        internal void UpdateTimer(DateTimeOffset timeStamp)
        {
            if (BuiltShip == null)
                TimeRemaining = null;
            else if (timeStamp > CompletionTime)
                TimeRemaining = default(TimeSpan);
            else
                TimeRemaining = CompletionTime - timeStamp;
        }
    }
}

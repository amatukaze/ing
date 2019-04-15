using System;

namespace Sakuno.ING.Game.Models
{
    public partial class BuildingDock
    {
        partial void UpdateCore(RawBuildingDock raw, DateTimeOffset timeStamp)
        {
            BuiltShip = owner.MasterData.ShipInfos[raw.BuiltShipId];
            Timer.Init(raw.CompletionTime, timeStamp);
        }

        internal void Instant()
        {
            State = BuildingDockState.BuildCompleted;
            Timer.Clear();
        }

        internal bool UpdateTimer(DateTimeOffset timeStamp) => Timer.Update(timeStamp);

        public CountDown Timer { get; } = new CountDown();
    }
}

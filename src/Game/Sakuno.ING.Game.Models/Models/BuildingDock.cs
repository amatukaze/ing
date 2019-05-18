using System;

namespace Sakuno.ING.Game.Models
{
    public partial class BuildingDock
    {
        partial void UpdateCore(RawBuildingDock raw, DateTimeOffset timeStamp)
        {
            BuiltShip = owner.MasterData.ShipInfos[raw.BuiltShipId];
            if (Timer.SetCompletionTime(raw.CompletionTime, timeStamp))
                owner.Notification.SetBuildCompletion(this, raw.CompletionTime);
        }

        internal void Instant()
        {
            State = BuildingDockState.BuildCompleted;
            Timer.Clear();
            owner.Notification.SetBuildCompletion(this, null);
        }

        internal void UpdateTimer(DateTimeOffset timeStamp) => Timer.Update(timeStamp);

        public CountDown Timer { get; } = new CountDown();
    }
}

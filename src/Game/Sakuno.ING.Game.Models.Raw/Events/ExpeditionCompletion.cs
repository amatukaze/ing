using Sakuno.ING.Game.Models;

namespace Sakuno.ING.Game.Events
{
    public class ExpeditionCompletion
    {
        public ExpeditionCompletion(FleetId fleetId, string expeditionName, ExpeditionResult result, Materials materialsAcquired, ItemRecord? rewardItem1, ItemRecord? rewardItem2)
        {
            FleetId = fleetId;
            ExpeditionName = expeditionName;
            Result = result;
            MaterialsAcquired = materialsAcquired;
            RewardItem1 = rewardItem1;
            RewardItem2 = rewardItem2;
        }

        public FleetId FleetId { get; }
        public string ExpeditionName { get; }
        public ExpeditionResult Result { get; }
        public Materials MaterialsAcquired { get; }
        public ItemRecord? RewardItem1 { get; }
        public ItemRecord? RewardItem2 { get; }
    }
}

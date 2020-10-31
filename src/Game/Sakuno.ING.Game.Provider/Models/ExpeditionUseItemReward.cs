using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Models
{
    public readonly struct ExpeditionUseItemReward
    {
        public UseItemId ItemId { get; }
        public int Count { get; }

        public ExpeditionUseItemReward(UseItemId itemId, int count)
        {
            ItemId = itemId;
            Count = count;
        }
    }
}

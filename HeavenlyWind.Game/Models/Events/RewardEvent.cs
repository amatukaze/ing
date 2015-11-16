using Sakuno.KanColle.Amatsukaze.Game.Models.Raw;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.Events
{
    public class RewardEvent : SortieEvent
    {
        public SortieItem Item { get; }
        public int Quantity { get; }

        internal RewardEvent(RawMapExploration rpData) : base(rpData)
        {
            Item = rpData.ItemReward.Item;
            Quantity = rpData.ItemReward.Quantity;
        }
    }
}

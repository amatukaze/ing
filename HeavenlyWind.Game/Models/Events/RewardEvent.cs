using Sakuno.KanColle.Amatsukaze.Game.Models.Raw;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.Events
{
    public class RewardEvent : RewardEventBase
    {
        public override int TypeID => RawData.Reward.TypeID;
        public override MaterialType ID => RawData.Reward.ID;
        public override int Quantity => RawData.Reward.Quantity;

        internal RewardEvent(RawMapExploration rpData) : base(rpData) { }
    }
}

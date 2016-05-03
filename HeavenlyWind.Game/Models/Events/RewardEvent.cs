using Sakuno.KanColle.Amatsukaze.Game.Models.Raw;
using System.Collections.Generic;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.Events
{
    public class RewardEvent : RewardEventBase
    {
        public override int TypeID => RawData.Rewards[0].TypeID;
        public override MaterialType ID => RawData.Rewards[0].ID;
        public override int Quantity => RawData.Rewards[0].Quantity;

        public IList<RawMapExploration.RawReward> Rewards { get; }

        internal RewardEvent(RawMapExploration rpData) : base(rpData)
        {
            Rewards = RawData.Rewards.AsReadOnly();
        }
    }
}

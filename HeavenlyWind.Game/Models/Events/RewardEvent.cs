using Newtonsoft.Json.Linq;
using Sakuno.KanColle.Amatsukaze.Game.Models.Raw;
using System.Collections.Generic;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.Events
{
    public class RewardEvent : RewardEventBase
    {
        public override int TypeID => Rewards[0].TypeID;
        public override MaterialType ID => Rewards[0].ID;
        public override int Quantity => Rewards[0].Quantity;

        public IList<RawMapExploration.RawReward> Rewards { get; }

        internal RewardEvent(RawMapExploration rpData) : base(rpData)
        {
            switch (RawData.Rewards.Type)
            {
                case JTokenType.Array:
                    Rewards = RawData.Rewards.ToObject<RawMapExploration.RawReward[]>();
                    break;

                case JTokenType.Object:
                    Rewards = new[] { RawData.Rewards.ToObject<RawMapExploration.RawReward>() };
                    break;
            }
        }
    }
}

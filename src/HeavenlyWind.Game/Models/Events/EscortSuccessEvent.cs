using Sakuno.KanColle.Amatsukaze.Game.Models.Raw;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.Events
{
    public class EscortSuccessEvent : RewardEventBase, IExtraInfo
    {
        public override int TypeID => RawData.RewardInExtraOperation.TypeID;
        public override MaterialType ID => RawData.RewardInExtraOperation.ID;
        public override int Quantity => RawData.RewardInExtraOperation.Quantity;

        internal EscortSuccessEvent(RawMapExploration rpData) : base(rpData) { }

        long IExtraInfo.GetExtraInfo() => ((int)ID << 16) + Quantity;
    }
}

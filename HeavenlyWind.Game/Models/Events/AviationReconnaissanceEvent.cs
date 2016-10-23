using Sakuno.KanColle.Amatsukaze.Game.Models.Raw;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.Events
{
    public enum AviationReconnaissancePlaneType { None, LargeFlyingBoat, Seaplane }
    public enum AviationReconnaissanceResult { Failure, Success, GreatSuccess }

    public class AviationReconnaissanceEvent : RewardEvent
    {
        public AviationReconnaissancePlaneType PlaneType => RawData.AviationReconnaissance.PlaneType;
        public AviationReconnaissanceResult Result => RawData.AviationReconnaissance.Result;

        public override int TypeID => Rewards != null ? Rewards[0].TypeID : -1;
        public override MaterialType ID => Rewards != null ? Rewards[0].ID : MaterialType.None;
        public override int Quantity => Rewards != null ? Rewards[0].Quantity : 0;

        internal AviationReconnaissanceEvent(RawMapExploration rpData) : base(rpData) { }
    }
}

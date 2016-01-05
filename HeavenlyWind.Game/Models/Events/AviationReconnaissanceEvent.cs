using Sakuno.KanColle.Amatsukaze.Game.Models.Raw;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.Events
{
    public enum AviationReconnaissancePlaneType { None, LargeFlyingBoat, Seaplane }
    public enum AviationReconnaissanceResult { Failure, Success, GreatSuccess }

    public class AviationReconnaissanceEvent : RewardEvent
    {
        public AviationReconnaissancePlaneType PlaneType => RawData.AviationReconnaissance.PlaneType;
        public AviationReconnaissanceResult Result => RawData.AviationReconnaissance.Result;

        internal AviationReconnaissanceEvent(RawMapExploration rpData) : base(rpData) { }
    }
}

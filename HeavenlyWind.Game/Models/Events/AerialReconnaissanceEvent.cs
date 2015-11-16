using Sakuno.KanColle.Amatsukaze.Game.Models.Raw;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.Events
{
    public enum AviationReconnaissancePlaneType { None, LargeFlyingBoat, Seaplane }
    public enum AviationReconnaissanceResult { Failure, Success, GreatSuccess }

    public class AerialReconnaissanceEvent : RewardEvent
    {
        public AviationReconnaissancePlaneType PlaneType { get; }
        public AviationReconnaissanceResult Result { get; }

        internal AerialReconnaissanceEvent(RawMapExploration rpData) : base(rpData)
        {
            PlaneType = rpData.AviationReconnaissance.PlaneType;
            Result = rpData.AviationReconnaissance.Result;
        }
    }
}

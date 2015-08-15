namespace Sakuno.KanColle.Amatsukaze.Game.Models.Events
{
    public enum AerialReconnaissancePlaneType { None, LargeSizedFlyingBoat, Seaplane }
    public enum AerialReconnaissanceResult { Failure, Success, GreatSuccess }

    public class AerialReconnaissanceEvent : RewardItemEvent
    {
        public AerialReconnaissancePlaneType PlaneType { get; }
        public AerialReconnaissanceResult Result { get; }

        internal AerialReconnaissanceEvent()
        {
        }
    }
}

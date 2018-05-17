using Sakuno.KanColle.Amatsukaze.Game.Models.Raw;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.Events
{
    public class LandingEvent : SortieEvent
    {
        public double TransportPoint { get; }

        internal LandingEvent(RawMapExploration rpData) : base(rpData)
        {
            var rSortie = SortieInfo.Current;

            var rTransportPoint = rSortie.Fleet.Status.TransportPoint;
            if (rSortie.EscortFleet != null)
                rTransportPoint += rSortie.EscortFleet.Status.TransportPoint;

            TransportPoint = rTransportPoint;
        }
    }
}

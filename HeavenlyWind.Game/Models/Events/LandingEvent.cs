using Sakuno.KanColle.Amatsukaze.Game.Models.Raw;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.Events
{
    public class LandingEvent : SortieEvent
    {
        internal LandingEvent(RawMapExploration rpData) : base(rpData)
        {
        }
    }
}

using Sakuno.KanColle.Amatsukaze.Game.Models.Raw;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.Events
{
    public class NothingHappenedEvent : SortieEvent
    {
        public string Message { get; }

        public bool CanManuallySelectRoute { get; }

        internal NothingHappenedEvent(RawMapExploration rpData) : base(rpData)
        {
        }
    }
}

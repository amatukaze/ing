using Sakuno.KanColle.Amatsukaze.Game.Models.Raw;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.Events
{
    public abstract class SortieEvent
    {
        protected RawMapExploration RawData { get; }

        internal protected SortieEvent(RawMapExploration rpData)
        {
            RawData = rpData;
        }
    }
}

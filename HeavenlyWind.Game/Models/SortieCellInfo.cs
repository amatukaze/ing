using Sakuno.KanColle.Amatsukaze.Game.Models.Events;

namespace Sakuno.KanColle.Amatsukaze.Game.Models
{
    public class SortieCellInfo
    {
        public int Cell { get; }

        public SortieEventType EventType { get; }
        public SortieEvent Event { get; }

        internal SortieCellInfo()
        {

        }
    }
}

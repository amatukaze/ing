using Sakuno.KanColle.Amatsukaze.Game.Models.Events;
using Sakuno.KanColle.Amatsukaze.Game.Models.Raw;

namespace Sakuno.KanColle.Amatsukaze.Game.Models
{
    public class SortieCellInfo
    {
        public int ID { get; }

        public SortieEventType EventType { get; }
        public SortieEvent Event { get; }

        internal SortieCellInfo(RawMapExploration rpData)
        {
            ID = rpData.Cell;
            EventType = rpData.CellEventType;
        }
    }
}

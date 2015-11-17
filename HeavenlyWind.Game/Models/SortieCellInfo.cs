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

            switch (EventType)
            {
                case SortieEventType.Reward:
                    Event = new RewardEvent(rpData);
                    break;

                case SortieEventType.Whirlpool:
                    Event = new WhirlpoolEvent(rpData);
                    break;

                case SortieEventType.NormalBattle:
                case SortieEventType.BossBattle:
                    Event = new BattleEvent(rpData);
                    break;

                case SortieEventType.NothingHappened:
                    Event = new NothingHappenedEvent(rpData);
                    break;

                case SortieEventType.AviationReconnaissance:
                    Event = new AviationReconnaissanceEvent(rpData);
                    break;

                case SortieEventType.EscortSuccess:
                    break;
            }
        }
    }
}

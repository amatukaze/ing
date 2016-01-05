using Sakuno.KanColle.Amatsukaze.Game.Models.Events;
using Sakuno.KanColle.Amatsukaze.Game.Models.Raw;

namespace Sakuno.KanColle.Amatsukaze.Game.Models
{
    public class SortieCellInfo
    {
        public int ID { get; }
        internal int InternalID { get; set; }

        public SortieEventType EventType { get; }
        public int EventSubType { get; }
        public SortieEvent Event { get; }

        public bool IsDeadEnd { get; }

        internal SortieCellInfo(RawMapExploration rpData)
        {
            ID = rpData.Cell;
            EventType = rpData.CellEventType;
            EventSubType = rpData.CellEventSubType;

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
                    Event = new EscortSuccessEvent(rpData);
                    break;

                case SortieEventType.Landing:
                    Event = new LandingEvent(rpData);
                    break;

            }

            IsDeadEnd = rpData.NextRouteCount == 0;
        }
    }
}

using Sakuno.KanColle.Amatsukaze.Game.Models.Events;
using Sakuno.KanColle.Amatsukaze.Game.Models.Raw;
using Sakuno.KanColle.Amatsukaze.Game.Services;
using System;

namespace Sakuno.KanColle.Amatsukaze.Game.Models
{
    public class SortieNodeInfo : ModelBase
    {
        public int ID { get; }
        internal int InternalID { get; }

        public string WikiID { get; }

        public SortieEventType EventType { get; }
        public int EventSubType { get; }
        public BattleType? BattleType => EventType == SortieEventType.NormalBattle ? (BattleType)EventSubType : (BattleType?)null;
        public SortieEvent Event { get; internal set; }

        public bool IsDeadEnd { get; }

        internal SortieNodeInfo(long rpTimestamp, MapInfo rpMap, RawMapExploration rpData)
        {
            ID = rpData.Node;

            var rDifficulty = rpMap.Difficulty;
            if (!rDifficulty.HasValue)
                InternalID = ID;
            else
            {
                var rDifficultyCount = Enum.GetNames(typeof(EventMapDifficulty)).Length - 1;
                InternalID = ID * rDifficultyCount + (int)rDifficulty.Value - 3;
            }

            WikiID = MapService.Instance.GetNodeWikiID(rpMap.ID, ID);

            EventType = rpData.NodeEventType;
            EventSubType = rpData.NodeEventSubType;

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
                    Event = new BattleEvent(rpTimestamp, rpMap, rpData, WikiID);
                    break;

                case SortieEventType.NothingHappened:
                    Event = new NothingHappenedEvent(rpMap, rpData);
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

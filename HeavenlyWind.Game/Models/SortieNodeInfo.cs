using Sakuno.KanColle.Amatsukaze.Game.Models.Events;
using Sakuno.KanColle.Amatsukaze.Game.Models.Raw;
using Sakuno.KanColle.Amatsukaze.Game.Services;
using System;

namespace Sakuno.KanColle.Amatsukaze.Game.Models
{
    public class SortieNodeInfo : ModelBase
    {
        SortieInfo r_Owner;

        public int ID { get; }

        public string WikiID { get; }

        public SortieEventType EventType { get; }
        public int EventSubType { get; }
        public BattleType? BattleType => EventType == SortieEventType.NormalBattle ? (BattleType)EventSubType : (BattleType?)null;
        public SortieEvent Event { get; internal set; }

        public bool IsDeadEnd { get; }

        public EnemyAerialRaid EnemyAerialRaid { get; }

        internal SortieNodeInfo(SortieInfo rpOwner, long rpTimestamp, RawMapExploration rpData)
        {
            r_Owner = rpOwner;

            ID = rpData.Node;

            var rMap = r_Owner.Map;

            WikiID = MapService.Instance.GetNodeWikiID(rMap.ID, ID);

            EventType = rpData.NodeEventType;
            EventSubType = rpData.NodeEventSubType;

            switch (EventType)
            {
                case SortieEventType.Reward:
                    Event = new RewardEvent(rpData);
                    break;

                case SortieEventType.Whirlpool:
                    Event = new WhirlpoolEvent(r_Owner.Fleet.Ships, r_Owner.EscortFleet, rpData);
                    break;

                case SortieEventType.NormalBattle:
                case SortieEventType.BossBattle:
                    Event = new BattleEvent(rpTimestamp, rMap, rpData, WikiID);
                    break;

                case SortieEventType.NothingHappened:
                    Event = new NothingHappenedEvent(rMap, rpData);
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

            if (rpData.EnemyAerialRaid != null)
                try
                {
                    EnemyAerialRaid = new EnemyAerialRaid(rpData.EnemyAerialRaid);

                    var rBattleEvent = Event as BattleEvent;
                    if (rBattleEvent != null)
                    {
                        rBattleEvent.EnemyAerialRaid = EnemyAerialRaid;
                        EnemyAerialRaid = null;
                    }
                }
                catch (Exception e)
                {
                    Logger.Write(LoggingLevel.Error, string.Format(StringResources.Instance.Main.Log_Exception_API_ParseException, e.Message));
                }
        }
    }
}

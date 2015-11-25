using System;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.Battle
{
    public class BattleInfo : ModelBase
    {
        static BattleInfo r_Current;

        public long ID { get; } = (long)DateTimeUtil.ToUnixTime(DateTimeOffset.Now);

        public bool IsInitialized { get; private set; }

        public BattleStage CurrentStage { get; private set; }
        public BattleStage First { get; private set; }
        public BattleStage Second { get; private set; }

        public Formation FriendFormation { get; private set; }
        public Formation EnemyFormation { get; private set; }
        public EngagementForm EngagementForm { get; private set; }

        public AerialCombat AerialCombat { get; } = new AerialCombat();

        public BattleResult Result { get; } = new BattleResult();

        internal BattleInfo()
        {
        }
    }
}

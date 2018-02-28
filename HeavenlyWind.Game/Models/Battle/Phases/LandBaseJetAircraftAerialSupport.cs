using Sakuno.KanColle.Amatsukaze.Game.Models.Raw.Battle;
using System;
using System.Collections.Generic;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.Battle.Phases
{
    class LandBaseJetAircraftAerialSupport : BattlePhase<RawAerialCombatPhase[]>
    {
        public LandBaseJetAircraftAerialSupport(BattleStage rpOwner, RawAerialCombatPhase[] rpRawData) : base(rpOwner, rpRawData)
        {
        }

        protected internal override void Process()
        {
            if (RawData == null)
                return;

            foreach (var rRawData in RawData)
                ProcessStage3(rRawData);
        }

        void ProcessStage3(RawAerialCombatPhase rpRawData)
        {
            ProcessStage3Core(Stage.EnemyMain, rpRawData.Stage3);
            ProcessStage3Core(Stage.EnemyEscort, rpRawData.Stage3CombinedFleet);
        }
        void ProcessStage3Core(IList<BattleParticipantSnapshot> rpParticipants, RawAerialCombatPhase.RawStage3 rpStage)
        {
            if (rpStage == null)
                return;

            var enemyMainCount = Math.Max(Stage.EnemyMain.Count, 6);

            for (var i = 0; i < Stage.Enemy.Count; i++)
            {
                BattleParticipantSnapshot participant;

                if (i < Stage.EnemyMain.Count)
                    participant = Stage.EnemyMain[i];
                else if (i >= enemyMainCount)
                    participant = Stage.EnemyEscort[i - enemyMainCount];
                else
                    continue;

                participant.Current -= rpStage.EnemyDamage[i];
            }
        }
    }
}

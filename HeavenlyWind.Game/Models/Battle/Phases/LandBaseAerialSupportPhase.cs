using Sakuno.KanColle.Amatsukaze.Game.Models.Raw.Battle;
using System;
using System.Collections.Generic;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.Battle.Phases
{
    class LandBaseAerialSupportPhase : BattlePhase<RawLandBaseAerialSupport[]>
    {
        internal LandBaseAerialSupportPhase(BattleStage rpOwner, RawLandBaseAerialSupport[] rpRawData) : base(rpOwner, rpRawData)
        {
        }

        internal protected override void Process()
        {
            if (RawData == null)
                return;

            var rInfo = Stage.Owner.AerialCombat;
            foreach (var rRawData in RawData)
            {
                ProcessStage1(rInfo, rRawData);
                ProcessStage2(rInfo, rRawData);
                ProcessStage3(rRawData);
            }
        }

        void ProcessStage1(AerialCombat rpInfo, RawLandBaseAerialSupport rpRawData)
        {
        }
        void ProcessStage2(AerialCombat rpInfo, RawLandBaseAerialSupport rpRawData)
        {
        }

        void ProcessStage3(RawLandBaseAerialSupport rpRawData)
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

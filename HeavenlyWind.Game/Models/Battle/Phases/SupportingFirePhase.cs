using Sakuno.KanColle.Amatsukaze.Game.Models.Raw.Battle;
using System;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.Battle.Phases
{
    class SupportingFirePhase : BattlePhase<RawSupportingFirePhase>
    {
        internal protected SupportingFirePhase(BattleStage rpOwner, RawSupportingFirePhase rpRawData) : base(rpOwner, rpRawData)
        {
        }

        internal protected override void Process()
        {
            if (RawData == null)
                return;

            var enemyMainCount = Math.Max(Stage.EnemyMain.Count, 6);
            var rEnemy = Stage.Enemy;

            var aerialSupportStage3 = RawData.AerialSupport?.Stage3;
            if (aerialSupportStage3 != null)
                for (var i = 0; i < Stage.Enemy.Count; i++)
                {
                    BattleParticipantSnapshot participant;

                    if (i < Stage.EnemyMain.Count)
                        participant = Stage.EnemyMain[i];
                    else if (i >= enemyMainCount)
                        participant = Stage.EnemyEscort[i - enemyMainCount];
                    else
                        continue;

                    participant.Current -= aerialSupportStage3.EnemyDamage[i];
                }

            var supportShelling = RawData.SupportShelling;
            if (supportShelling != null)
                for (var i = 0; i < Stage.Enemy.Count; i++)
                {
                    BattleParticipantSnapshot participant;

                    if (i < Stage.EnemyMain.Count)
                        participant = Stage.EnemyMain[i];
                    else if (i >= enemyMainCount)
                        participant = Stage.EnemyEscort[i - enemyMainCount];
                    else
                        continue;

                    participant.Current -= supportShelling.Damage[i];
                }
        }
    }
}

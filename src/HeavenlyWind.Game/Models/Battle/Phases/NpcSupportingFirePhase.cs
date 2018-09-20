using Newtonsoft.Json.Linq;
using Sakuno.KanColle.Amatsukaze.Game.Models.Raw.Battle;
using System;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.Battle.Phases
{
    class NpcSupportingFirePhase : BattlePhase<RawShellingPhase>
    {
        internal protected NpcSupportingFirePhase(BattleStage rpOwner, RawShellingPhase rpRawData) : base(rpOwner, rpRawData)
        {
        }

        internal protected override void Process()
        {
            if (RawData == null || RawData.Attackers == null)
                return;

            var rParticipants = Stage.FriendAndEnemy;
            var rAttackers = RawData.Attackers;
            var rIsEnemyAttacker = RawData.IsEnemyAttacker;
            for (var i = 0; i < rAttackers.Length; i++)
            {
                var rDefenders = ((JArray)RawData.Defenders[i]).ToObject<int[]>();
                var rDamages = ((JArray)RawData.Damages[i]).ToObject<int[]>();

                var rIsEnemy = rIsEnemyAttacker[i] == 1;

                for (var j = 0; j < rDefenders.Length; j++)
                {
                    if (rDefenders[j] == -1)
                        continue;

                    var rDamage = rDamages[j];

                    var rDefenderIndex = rDefenders[j];

                    BattleParticipantSnapshot rDefender;

                    var enemyMainCount = Math.Max(Stage.EnemyMain.Count, 6);

                    if (rIsEnemy)
                        continue;

                    if (rDefenderIndex < enemyMainCount)
                        rDefender = Stage.EnemyMain[rDefenderIndex];
                    else
                        rDefender = Stage.EnemyEscort[rDefenderIndex - enemyMainCount];

                    rDefender.Current -= rDamage;
                }
            }
        }
    }
}

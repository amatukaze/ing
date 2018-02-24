using Newtonsoft.Json.Linq;
using Sakuno.KanColle.Amatsukaze.Game.Models.Raw.Battle;
using System;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.Battle.Phases
{
    class ShellingPhase : BattlePhase<RawShellingPhase>
    {
        internal ShellingPhase(BattleStage rpStage, RawShellingPhase rpRawData) : base(rpStage, rpRawData)
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
                    var rAttackerIndex = rAttackers[i];

                    BattleParticipantSnapshot rDefender, rAttacker;

                    var enemyMainCount = Math.Max(Stage.EnemyMain.Count, 6);
                    var friendMainCount = Math.Max(Stage.FriendMain.Count, 6);

                    if (!rIsEnemy)
                    {
                        if (rDefenderIndex < enemyMainCount)
                            rDefender = Stage.EnemyMain[rDefenderIndex];
                        else
                            rDefender = Stage.EnemyEscort[rDefenderIndex - enemyMainCount];

                        if (rAttackerIndex < friendMainCount)
                            rAttacker = Stage.FriendMain[rAttackerIndex];
                        else
                            rAttacker = Stage.FriendEscort[rAttackerIndex - friendMainCount];
                    }
                    else
                    {
                        if (rDefenderIndex < friendMainCount)
                            rDefender = Stage.FriendMain[rDefenderIndex];
                        else
                            rDefender = Stage.FriendEscort[rDefenderIndex - friendMainCount];

                        if (rAttackerIndex < enemyMainCount)
                            rAttacker = Stage.EnemyMain[rAttackerIndex];
                        else
                            rAttacker = Stage.EnemyEscort[rAttackerIndex - enemyMainCount];
                    }

                    rDefender.Current -= rDamage;
                    rAttacker.DamageGivenToOpponent += rDamage;
                }
            }
        }
    }
}

using Newtonsoft.Json.Linq;
using Sakuno.KanColle.Amatsukaze.Game.Models.Raw.Battle;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.Battle.Phases
{
    public class OpeningASWPhase : BattlePhase<RawOpeningASWPhase>
    {
        bool r_IsFriendEscortFleet;
        bool r_IsEnemyEscortFleet;

        internal OpeningASWPhase(BattleStage rpStage, RawOpeningASWPhase rpRawData, bool rpIsFriendEscortFleet = false, bool rpIsEnemyEscortFleet = false) : base(rpStage, rpRawData)
        {
            r_IsFriendEscortFleet = rpIsFriendEscortFleet;
            r_IsEnemyEscortFleet = rpIsEnemyEscortFleet;
        }

        int GetIndex(int rpPosition)
        {
            if ((r_IsFriendEscortFleet && rpPosition < 6) || (r_IsEnemyEscortFleet && rpPosition >= 6))
                return rpPosition + 12;

            return rpPosition;
        }

        protected internal override void Process()
        {
            if (RawData == null)
                return;

            var rParticipants = Stage.FriendAndEnemy;
            var rAttackers = RawData.Attackers;
            var rIsEnemyAttacker = RawData.IsEnemyAttacker;
            for (var i = 0; i < rAttackers.Length; i++)
            {
                var rDefenders = ((JArray)RawData.Defenders[i]).ToObject<int[]>();
                var rDamages = ((JArray)RawData.Damages[i]).ToObject<int[]>();

                if (rIsEnemyAttacker == null)
                    for (var j = 0; j < rDefenders.Length; j++)
                    {
                        if (rDefenders[j] == -1)
                            continue;

                        var rDamage = rDamages[j];
                        rParticipants[GetIndex(rDefenders[j])].Current -= rDamage;
                        rParticipants[GetIndex(rAttackers[i])].DamageGivenToOpponent += rDamage;
                    }
                else
                {
                    var rIsEnemy = rIsEnemyAttacker[i] == 1;

                    for (var j = 0; j < rDefenders.Length; j++)
                    {
                        if (rDefenders[j] == -1)
                            continue;

                        var rDamage = rDamages[j];

                        if (!rIsEnemy)
                        {
                            Stage.Enemy[rDefenders[j]].Current -= rDamage;
                            Stage.Friend[rAttackers[i]].DamageGivenToOpponent += rDamage;
                        }
                        else
                        {
                            Stage.Friend[rDefenders[j]].Current -= rDamage;
                            Stage.Enemy[rAttackers[i]].DamageGivenToOpponent += rDamage;
                        }
                    }
                }
            }
        }
    }
}

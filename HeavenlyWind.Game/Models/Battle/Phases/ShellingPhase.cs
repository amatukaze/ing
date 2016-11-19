using Newtonsoft.Json.Linq;
using Sakuno.KanColle.Amatsukaze.Game.Models.Raw.Battle;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.Battle.Phases
{
    class ShellingPhase : BattlePhase<RawShellingPhase>
    {
        bool r_IsFriendEscortFleet;
        bool r_IsEnemyEscortFleet;

        public int[] ParticipantingFleet { get; set; }

        internal ShellingPhase(BattleStage rpStage, RawShellingPhase rpRawData, bool rpIsFriendEscortFleet = false, bool rpIsEnemyEscortFleet = false) : base(rpStage, rpRawData)
        {
            r_IsFriendEscortFleet = rpIsFriendEscortFleet;
            r_IsEnemyEscortFleet = rpIsEnemyEscortFleet;
        }

        int GetIndex(int rpPosition)
        {
            if ((r_IsFriendEscortFleet && rpPosition <= 6) || (r_IsEnemyEscortFleet && rpPosition > 6) || (ParticipantingFleet != null && ParticipantingFleet[1] == 2 && rpPosition > 6))
                return rpPosition + 12 - 1;

            return rpPosition - 1;
        }

        internal protected override void Process()
        {
            if (RawData == null)
                return;

            var rParticipants = Stage.FriendAndEnemy;
            var rAttackers = RawData.Attackers;
            var rIsEnemyAttacker = RawData.IsEnemyAttacker;
            for (var i = 1; i < rAttackers.Length; i++)
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
                            Stage.Enemy[rDefenders[j] - 1].Current -= rDamage;
                            Stage.Friend[rAttackers[i] - 1].DamageGivenToOpponent += rDamage;
                        }
                        else
                        {
                            Stage.Friend[rDefenders[j] - 1].Current -= rDamage;
                            Stage.Enemy[rAttackers[i] - 1].DamageGivenToOpponent += rDamage;
                        }
                    }
                }
            }
        }
    }
}

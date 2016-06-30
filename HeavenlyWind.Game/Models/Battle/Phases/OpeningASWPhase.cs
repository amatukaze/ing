using Newtonsoft.Json.Linq;
using Sakuno.KanColle.Amatsukaze.Game.Models.Raw.Battle;
using System;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.Battle.Phases
{
    public class OpeningASWPhase : BattlePhase<RawOpeningASWPhase>
    {
        bool r_IsEscortFleet;

        internal OpeningASWPhase(BattleStage rpStage, RawOpeningASWPhase rpRawData, bool rpIsEscortFleet = false) : base(rpStage, rpRawData)
        {
            r_IsEscortFleet = rpIsEscortFleet;
        }

        protected internal override void Process()
        {
            if (RawData == null)
                return;

            Func<int, int> rIndex = r => r_IsEscortFleet && r <= 6 ? r + 12 - 1 : r - 1;

            var rParticipants = Stage.FriendAndEnemy;
            var rAttackers = RawData.Attackers;
            for (var i = 1; i < rAttackers.Length; i++)
            {
                var rDefenders = ((JArray)RawData.Defenders[i]).ToObject<int[]>();
                var rDamages = ((JArray)RawData.Damages[i]).ToObject<int[]>();

                for (var j = 0; j < rDefenders.Length; j++)
                {
                    if (rDefenders[j] == -1)
                        continue;

                    var rDamage = rDamages[j];
                    rParticipants[rIndex(rDefenders[j])].Current -= rDamage;
                    rParticipants[rIndex(rAttackers[i])].DamageGivenToOpponent += rDamage;
                }
            }
        }
    }
}

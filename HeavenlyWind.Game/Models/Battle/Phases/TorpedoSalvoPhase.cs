using Sakuno.KanColle.Amatsukaze.Game.Models.Raw.Battle;
using System;
using System.Linq;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.Battle.Phases
{
    class TorpedoSalvoPhase : BattlePhase<RawTorpedoSalvoPhase>
    {
        bool r_IsEscortFleet;

        internal TorpedoSalvoPhase(BattleStage rpStage, RawTorpedoSalvoPhase rpRawData, bool rpIsEscortFleet = false) : base(rpStage, rpRawData)
        {
            r_IsEscortFleet = rpIsEscortFleet;
        }

        protected internal override void Process()
        {
            if (RawData == null)
                return;

            Func<int, int> rIndex = r => r_IsEscortFleet && r < 6 ? r + 12 : r;

            var rParticipants = Stage.FriendAndEnemy;

            var rDamages = Enumerable.Concat(RawData.FriendDamage.Skip(1), RawData.EnemyDamage.Skip(1)).ToArray();
            for (var i = 0; i < rDamages.Length; i++)
            {
                var rParticipant = rParticipants[rIndex(i)];
                if (rParticipant != null)
                    rParticipant.Current -= rDamages[i];
            }

            var rDamagesGivenToOpponent = Enumerable.Concat(RawData.FriendDamageGivenToOpponent.Skip(1), RawData.EnemyDamageGivenToOpponent.Skip(1)).ToArray();
            for (var i = 0; i < rDamagesGivenToOpponent.Length; i++)
            {
                var rParticipant = rParticipants[rIndex(i)];
                if (rParticipant != null)
                    rParticipant.DamageGivenToOpponent += rDamagesGivenToOpponent[i];
            }
        }
    }
}

using Sakuno.KanColle.Amatsukaze.Game.Models.Raw.Battle;
using System.Linq;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.Battle.Phases
{
    class TorpedoSalvoPhase : BattlePhase<RawTorpedoSalvoPhase>
    {
        bool r_IsFriendEscortFleet;
        bool r_IsEnemyEscortFleet;

        internal TorpedoSalvoPhase(BattleStage rpStage, RawTorpedoSalvoPhase rpRawData, bool rpIsFriendEscortFleet = false, bool rpIsEnemyEscortFleet = false) : base(rpStage, rpRawData)
        {
            r_IsFriendEscortFleet = rpIsFriendEscortFleet;
            r_IsEnemyEscortFleet = rpIsEnemyEscortFleet;
        }

        int GetIndex(int rpPosition)
        {
            if (r_IsFriendEscortFleet && !r_IsEnemyEscortFleet && rpPosition < 7)
                return rpPosition + 14;

            return rpPosition;
        }

        protected internal override void Process()
        {
            if (RawData == null)
                return;

            var rParticipants = Stage.FriendAndEnemy;

            var rFriendDamages = RawData.FriendDamage;
            var rFriendMainDamages = rFriendDamages.Take(7);
            var rFriendEscortDamages = rFriendDamages.Skip(7);
            var rEnemyDamages = RawData.EnemyDamage;
            var rEnemyMainDamages = rEnemyDamages.Take(7);
            var rEnemyEscortDamages = rEnemyDamages.Skip(7);
            var rDamages = Enumerable.Concat(rFriendMainDamages, rEnemyMainDamages).Concat(Enumerable.Concat(rFriendEscortDamages, rEnemyEscortDamages)).ToArray();
            for (var i = 0; i < rDamages.Length; i++)
            {
                var rParticipant = rParticipants[GetIndex(i)];
                if (rParticipant != null)
                    rParticipant.Current -= rDamages[i];
            }

            var rFriendDamagesGivenToOpponent = RawData.FriendDamageGivenToOpponent;
            var rFriendMainDamagesGivenToOpponent = rFriendDamagesGivenToOpponent.Take(7);
            var rFriendEscortDamagesGivenToOpponent = rFriendDamagesGivenToOpponent.Skip(7);
            var rEnemyDamagesGivenToOpponent = RawData.EnemyDamageGivenToOpponent;
            var rEnemyMainDamagesGivenToOpponent = rEnemyDamagesGivenToOpponent.Take(7);
            var rEnemyEscortDamagesGivenToOpponent = rEnemyDamagesGivenToOpponent.Skip(7);
            var rDamagesGivenToOpponent = Enumerable.Concat(rFriendMainDamagesGivenToOpponent, rEnemyMainDamagesGivenToOpponent).Concat(Enumerable.Concat(rFriendEscortDamagesGivenToOpponent, rEnemyEscortDamagesGivenToOpponent)).ToArray();
            for (var i = 0; i < rDamagesGivenToOpponent.Length; i++)
            {
                var rParticipant = rParticipants[GetIndex(i)];
                if (rParticipant != null)
                    rParticipant.DamageGivenToOpponent += rDamagesGivenToOpponent[i];
            }
        }
    }
}

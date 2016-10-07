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

        int GetIndex(int rpPosition)
        {
            if (r_IsEscortFleet && rpPosition < 6)
                return rpPosition + 12;

            return rpPosition;
        }

        protected internal override void Process()
        {
            if (RawData == null)
                return;

            var rParticipants = Stage.FriendAndEnemy;

            var rFriendDamages = RawData.FriendDamage.Skip(1);
            var rFriendMainDamages = rFriendDamages.Take(6);
            var rFriendEscortDamages = rFriendDamages.Skip(6);
            var rEnemyDamages = RawData.EnemyDamage.Skip(1);
            var rEnemyMainDamages = rEnemyDamages.Take(6);
            var rEnemyEscortDamages = rEnemyDamages.Skip(6);
            var rDamages = Enumerable.Concat(rFriendMainDamages, rEnemyMainDamages).Concat(Enumerable.Concat(rFriendEscortDamages, rEnemyEscortDamages)).ToArray();
            for (var i = 0; i < rDamages.Length; i++)
            {
                var rParticipant = rParticipants[GetIndex(i)];
                if (rParticipant != null)
                    rParticipant.Current -= rDamages[i];
            }

            var rFriendDamagesGivenToOpponent = RawData.FriendDamageGivenToOpponent.Skip(1);
            var rFriendMainDamagesGivenToOpponent = rFriendDamagesGivenToOpponent.Take(6);
            var rFriendEscortDamagesGivenToOpponent = rFriendDamagesGivenToOpponent.Skip(6);
            var rEnemyDamagesGivenToOpponent = RawData.EnemyDamageGivenToOpponent.Skip(1);
            var rEnemyMainDamagesGivenToOpponent = rEnemyDamagesGivenToOpponent.Take(6);
            var rEnemyEscortDamagesGivenToOpponent = rEnemyDamagesGivenToOpponent.Skip(6);
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

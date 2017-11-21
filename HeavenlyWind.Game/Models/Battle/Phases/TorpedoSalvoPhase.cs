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
            if (r_IsFriendEscortFleet && !r_IsEnemyEscortFleet && rpPosition < 6)
                return rpPosition + 12;

            return rpPosition;
        }

        protected internal override void Process()
        {
            if (RawData == null)
                return;

            var rParticipants = Stage.FriendAndEnemy;

            var count = Stage.FriendMain.Count == 7 ? 7 : 6;

            var rFriendDamages = RawData.FriendDamage;
            var rFriendMainDamages = rFriendDamages.Take(count);
            var rFriendEscortDamages = rFriendDamages.Skip(count);
            var rEnemyDamages = RawData.EnemyDamage;
            var rEnemyMainDamages = rEnemyDamages.Take(count);
            var rEnemyEscortDamages = rEnemyDamages.Skip(count);

            if (count == 6)
            {
                var pad = Enumerable.Repeat(-1, 1);

                rFriendMainDamages = rFriendMainDamages.Concat(pad);
                rFriendEscortDamages = rFriendEscortDamages.Concat(pad);
                rEnemyMainDamages = rEnemyMainDamages.Concat(pad);
                rEnemyEscortDamages = rEnemyEscortDamages.Concat(pad);
            }

            var rDamages = Enumerable.Concat(rFriendMainDamages, rEnemyMainDamages).Concat(Enumerable.Concat(rFriendEscortDamages, rEnemyEscortDamages)).ToArray();
            for (var i = 0; i < rDamages.Length; i++)
            {
                var rParticipant = rParticipants[GetIndex(i)];
                if (rParticipant != null)
                    rParticipant.Current -= rDamages[i];
            }

            var rFriendDamagesGivenToOpponent = RawData.FriendDamageGivenToOpponent;
            var rFriendMainDamagesGivenToOpponent = rFriendDamagesGivenToOpponent.Take(count);
            var rFriendEscortDamagesGivenToOpponent = rFriendDamagesGivenToOpponent.Skip(count);
            var rEnemyDamagesGivenToOpponent = RawData.EnemyDamageGivenToOpponent;
            var rEnemyMainDamagesGivenToOpponent = rEnemyDamagesGivenToOpponent.Take(count);
            var rEnemyEscortDamagesGivenToOpponent = rEnemyDamagesGivenToOpponent.Skip(count);

            if (count == 6)
            {
                var pad = Enumerable.Repeat(-1, 1);

                rFriendMainDamagesGivenToOpponent = rFriendMainDamagesGivenToOpponent.Concat(pad);
                rFriendEscortDamagesGivenToOpponent = rFriendEscortDamagesGivenToOpponent.Concat(pad);
                rEnemyMainDamagesGivenToOpponent = rEnemyMainDamagesGivenToOpponent.Concat(pad);
                rEnemyEscortDamagesGivenToOpponent = rEnemyEscortDamagesGivenToOpponent.Concat(pad);
            }

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

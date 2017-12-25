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

            for (var i = 0; i < Stage.Friend.Count;i++)
            {
                var participant = Stage.Friend[i];

                participant.Current -= RawData.FriendDamage[i];
                participant.DamageGivenToOpponent += RawData.FriendDamageGivenToOpponent[i];
            }

            for (var i = 0; i < Stage.Enemy.Count; i++)
                Stage.Enemy[i].Current -= RawData.EnemyDamage[i];
        }
    }
}

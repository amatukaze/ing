using Sakuno.KanColle.Amatsukaze.Game.Models.Raw.Battle;
using System;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.Battle.Phases
{
    class TorpedoSalvoPhase : BattlePhase<RawTorpedoSalvoPhase>
    {
        internal TorpedoSalvoPhase(BattleStage rpStage, RawTorpedoSalvoPhase rpRawData) : base(rpStage, rpRawData)
        {
        }

        protected internal override void Process()
        {
            if (RawData == null)
                return;

            var friendMainCount = Math.Max(Stage.FriendMain.Count, 6);

            for (var i = 0; i < Stage.Friend.Count;i++)
            {
                BattleParticipantSnapshot participant;

                if (i < Stage.FriendMain.Count)
                    participant = Stage.FriendMain[i];
                else if (i >= friendMainCount)
                    participant = Stage.FriendEscort[i - friendMainCount];
                else
                    continue;

                participant.Current -= RawData.FriendDamage[i];
                participant.DamageGivenToOpponent += RawData.FriendDamageGivenToOpponent[i];
            }

            var enemyMainCount = Math.Max(Stage.EnemyMain.Count, 6);

            for (var i = 0; i < Stage.Enemy.Count; i++)
            {
                BattleParticipantSnapshot participant;

                if (i < Stage.EnemyMain.Count)
                    participant = Stage.EnemyMain[i];
                else if (i >= enemyMainCount)
                    participant = Stage.EnemyEscort[i - enemyMainCount];
                else
                    continue;

                participant.Current -= RawData.EnemyDamage[i];
                participant.DamageGivenToOpponent += RawData.EnemyDamageGivenToOpponent[i];
            }
        }
    }
}

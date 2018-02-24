using Sakuno.KanColle.Amatsukaze.Game.Models.Raw.Battle;

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

            for (var i = 0; i < Stage.Friend.Count;i++)
            {
                var participant = Stage.Friend[i];

                participant.Current -= RawData.FriendDamage[i];
                participant.DamageGivenToOpponent += RawData.FriendDamageGivenToOpponent[i];
            }

            for (var i = 0; i < Stage.Enemy.Count; i++)
            {
                var participant = Stage.Enemy[i];

                participant.Current -= RawData.EnemyDamage[i];
                participant.DamageGivenToOpponent += RawData.EnemyDamageGivenToOpponent[i];
            }
        }
    }
}

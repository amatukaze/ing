using Sakuno.KanColle.Amatsukaze.Game.Models.Raw.Battle;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.Battle.Phases
{
    class SupportingFirePhase : BattlePhase<RawSupportingFirePhase>
    {
        internal protected SupportingFirePhase(BattleStage rpOwner, RawSupportingFirePhase rpRawData) : base(rpOwner, rpRawData)
        {
        }

        internal protected override void Process()
        {
            if (RawData == null)
                return;

            var rEnemy = Stage.Enemy;
            var rShelling = RawData.Shelling;
            if (rShelling != null)
                for (var i = 0; i < rEnemy.Count; i++)
                    rEnemy[i].Current -= rShelling.Damage[i + 1];
        }
    }
}

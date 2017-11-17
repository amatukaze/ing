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

            var rAerialSupport = RawData.AerialSupport;
            if (rAerialSupport != null)
            {
                var rStage3 = rAerialSupport.Stage3;
                if (rStage3 != null)
                    for (var i = 0; i < rEnemy.Count; i++)
                        rEnemy[i].Current -= rStage3.EnemyDamage[i];
            }

            var rSupportShelling = RawData.SupportShelling;
            if (rSupportShelling != null)
                for (var i = 0; i < rEnemy.Count; i++)
                    rEnemy[i].Current -= rSupportShelling.Damage[i];
        }
    }
}

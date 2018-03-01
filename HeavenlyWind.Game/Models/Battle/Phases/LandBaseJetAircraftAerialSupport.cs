using Sakuno.KanColle.Amatsukaze.Game.Models.Raw.Battle;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.Battle.Phases
{
    class LandBaseJetAircraftAerialSupport : BattlePhase<RawAerialCombatPhase[]>
    {
        public LandBaseJetAircraftAerialSupport(BattleStage rpOwner, RawAerialCombatPhase[] rpRawData) : base(rpOwner, rpRawData)
        {
        }

        protected internal override void Process()
        {
            if (RawData == null)
                return;

            foreach (var rRawData in RawData)
                ProcessStage3(rRawData);
        }

        void ProcessStage3(RawAerialCombatPhase rpRawData)
        {
            var stage3 = rpRawData.Stage3;
            if (stage3 == null)
                return;

            var enemyDamage = stage3.EnemyDamage;
            if (enemyDamage != null)
                for (var i = 0; i < Stage.EnemyMain.Count; i++)
                {
                    var damage = stage3.EnemyDamage[i];

                    Stage.EnemyMain[i].Current -= damage;
                }

            var enemyCombinedFleetDamages = rpRawData.Stage3CombinedFleet?.EnemyDamage;
            if (enemyCombinedFleetDamages != null)
                for (var i = 0; i < Stage.EnemyEscort.Count; i++)
                {
                    var damage = enemyCombinedFleetDamages[i];

                    Stage.EnemyEscort[i].Current -= damage;
                }
        }
    }
}

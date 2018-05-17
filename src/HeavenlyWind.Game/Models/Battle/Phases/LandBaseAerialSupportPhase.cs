using Sakuno.KanColle.Amatsukaze.Game.Models.Raw.Battle;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.Battle.Phases
{
    class LandBaseAerialSupportPhase : BattlePhase<RawLandBaseAerialSupport[]>
    {
        internal LandBaseAerialSupportPhase(BattleStage rpOwner, RawLandBaseAerialSupport[] rpRawData) : base(rpOwner, rpRawData)
        {
        }

        internal protected override void Process()
        {
            if (RawData == null)
                return;

            var rInfo = Stage.Owner.AerialCombat;
            foreach (var rRawData in RawData)
            {
                ProcessStage1(rInfo, rRawData);
                ProcessStage2(rInfo, rRawData);
                ProcessStage3(rRawData);
            }
        }

        void ProcessStage1(AerialCombat rpInfo, RawLandBaseAerialSupport rpRawData)
        {
        }
        void ProcessStage2(AerialCombat rpInfo, RawLandBaseAerialSupport rpRawData)
        {
        }

        void ProcessStage3(RawLandBaseAerialSupport rpRawData)
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

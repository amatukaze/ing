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
            var rStage3 = rpRawData.Stage3;
            if (rStage3 == null)
                return;

            var rParticipants = Stage.Enemy;
            var rEnemyDamages = rStage3.EnemyDamage;
            for (var i = 0; i < rParticipants.Count; i++)
            {
                var rParticipant = rParticipants[i];
                if (rParticipant != null)
                    rParticipant.Current -= rEnemyDamages[i + 1];
            }
        }
    }
}

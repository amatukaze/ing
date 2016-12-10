using Sakuno.KanColle.Amatsukaze.Game.Models.Raw.Battle;
using System.Collections.Generic;

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
            ProcessStage3Core(Stage.EnemyMain, rpRawData.Stage3);
            ProcessStage3Core(Stage.EnemyEscort, rpRawData.Stage3CombinedFleet);
        }
        void ProcessStage3Core(IList<BattleParticipantSnapshot> rpParticipants, RawAerialCombatPhase.RawStage3 rpStage)
        {
            if (rpStage == null)
                return;

            var rParticipants = Stage.EnemyMain;
            var rEnemyDamages = rpStage.EnemyDamage;

            for (var i = 0; i < rpParticipants.Count; i++)
            {
                var rParticipant = rpParticipants[i];
                if (rParticipant != null)
                    rParticipant.Current -= rEnemyDamages[i + 1];
            }
        }
    }
}

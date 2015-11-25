using Sakuno.KanColle.Amatsukaze.Game.Models.Raw.Battle;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.Battle.Phases
{
    class AerialCombatPhase : BattlePhase<RawAerialCombatPhase>
    {
        public PhaseRound Round { get; }

        internal AerialCombatPhase(BattleStage rpStage, RawAerialCombatPhase rpRawData, PhaseRound rpRound = PhaseRound.First) : base(rpStage, rpRawData)
        {
            Round = rpRound;
        }

        internal protected override void Process()
        {
            var rInfo = Stage.Owner.AerialCombat;

            ProcessStage1(rInfo);
            ProcessStage2(rInfo);
            ProcessStage3();
        }

        void ProcessStage1(AerialCombat rpInfo)
        {
            var rStage1 = RawData.Stage1;
            if (rStage1 != null)
            {
            }
        }

        void ProcessStage2(AerialCombat rpInfo)
        {
            var rStage2 = RawData.Stage2;
            if (rStage2 != null)
            {
            }
        }

        void ProcessStage3()
        {
            var rStage3 = RawData.Stage3;
            if (rStage3 == null)
                return;
        }
    }
}

using Sakuno.KanColle.Amatsukaze.Game.Models.Battle.Phases;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.Battle.Stages
{
    abstract class Day : BattleStage
    {
        public LandBaseJetAircraftAerialSupport LandBaseJetAircraftAerialSupport { get; protected set; }
        public AerialCombatPhase JetAircraftAerialCombat { get; protected set; }
        public LandBaseAerialSupportPhase LandBaseAerialSupport { get; protected set; }
        public AerialCombatPhase AerialCombat { get; protected set; }
        public SupportingFirePhase SupportingFire { get; protected set; }
        public ShellingPhase OpeningASW { get; protected set; }
        public TorpedoSalvoPhase OpeningTorpedo { get; protected set; }

        public ShellingPhase ShellingFirstRound { get; protected set; }
        public ShellingPhase ShellingSecondRound { get; protected set; }

        public TorpedoSalvoPhase ClosingTorpedo { get; protected set; }

        internal protected Day(BattleInfo rpOwner) : base(rpOwner) { }
    }
}

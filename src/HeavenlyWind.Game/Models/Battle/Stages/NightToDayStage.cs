using Sakuno.KanColle.Amatsukaze.Game.Models.Battle.Phases;
using Sakuno.KanColle.Amatsukaze.Game.Models.Raw.Battle;
using Sakuno.KanColle.Amatsukaze.Game.Parsers;
using System.Collections.Generic;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.Battle.Stages
{
    class NightToDayStage : Day
    {
        public override BattleStageType Type => BattleStageType.NightToDay;

        public SupportingFirePhase NightSupportingFire { get; protected set; }

        public ShellingPhase NightShellingFirstRound { get; protected set; }
        public ShellingPhase NightShellingSecondRound { get; protected set; }

        public override IList<BattlePhase> Phases => new BattlePhase[]
        {
            NightSupportingFire,

            NightShellingFirstRound,
            NightShellingSecondRound,

            LandBaseJetAircraftAerialSupport,
            JetAircraftAerialCombat,
            LandBaseAerialSupport,
            AerialCombat,
            SupportingFire,
            OpeningASW,
            OpeningTorpedo,

            ShellingFirstRound,
            ShellingSecondRound,
            ClosingTorpedo,
        };

        internal protected NightToDayStage(BattleInfo rpOwner, ApiInfo rpInfo) : base(rpOwner)
        {
            var rRawData = rpInfo.Data as RawNightToDay;

            NightSupportingFire = new SupportingFirePhase(this, rRawData.NightSupportingFire);

            NightShellingFirstRound = new ShellingPhase(this, rRawData.NightShellingFirstRound);
            NightShellingSecondRound = new ShellingPhase(this, rRawData.NightShellingSecondRound);

            LandBaseJetAircraftAerialSupport = new LandBaseJetAircraftAerialSupport(this, rRawData.LandBaseJetAircraftAerialSupport);
            JetAircraftAerialCombat = new AerialCombatPhase(this, rRawData.JetAircraftAerialCombat);
            LandBaseAerialSupport = new LandBaseAerialSupportPhase(this, rRawData.LandBaseAerialSupport);
            AerialCombat = new AerialCombatPhase(this, rRawData.AerialCombat);
            SupportingFire = new SupportingFirePhase(this, rRawData.SupportingFire);
            OpeningASW = new ShellingPhase(this, rRawData.OpeningASW);
            OpeningTorpedo = new OpeningTorpedoSalvoPhase(this, rRawData.OpeningTorpedoSalvo);

            ShellingFirstRound = new ShellingPhase(this, rRawData.ShellingFirstRound);
            ShellingSecondRound = new ShellingPhase(this, rRawData.ShellingSecondRound);
            ClosingTorpedo = new TorpedoSalvoPhase(this, rRawData.ClosingTorpedoSalvo);
        }
    }
}

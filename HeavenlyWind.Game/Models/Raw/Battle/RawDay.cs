using Newtonsoft.Json;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.Raw.Battle
{
    public class RawDay : RawBattleBase, IRawFormationAndEngagementForm
    {
        [JsonProperty("api_dock_id")]
        public override int FleetID { get; set; }

        [JsonProperty("api_midnight_flag")]
        public bool IsNightBattleAvailable { get; set; }

        [JsonProperty("api_search")]
        public int[] DetectionResult { get; set; }

        [JsonProperty("api_formation")]
        public int[] FormationAndEngagementForm { get; set; }

        [JsonProperty("api_air_base_attack")]
        public RawLandBaseAerialSupport[] LandBaseAerialSupport { get; set; }

        //[JsonProperty("api_stage_flag")]
        [JsonProperty("api_kouku")]
        public RawAerialCombatPhase AerialCombat { get; set; }

        [JsonProperty("api_support_flag")]
        public int SupportingFireType { get; set; }
        [JsonProperty("api_support_info")]
        public RawSupportingFirePhase SupportingFire { get; set; }

        [JsonProperty("api_opening_taisen_flag")]
        public bool IsOpeningASWAvailable { get; set; }
        [JsonProperty("api_opening_taisen")]
        public RawOpeningASWPhase OpeningASW { get; set; }

        [JsonProperty("api_opening_flag")]
        public bool IsOpeningTorpedoSalvoAvailable { get; set; }
        [JsonProperty("api_opening_atack")]
        public RawTorpedoSalvoPhase OpeningTorpedoSalvo { get; set; }

        //[JsonProperty("api_hourai_flag")]

        [JsonProperty("api_hougeki1")]
        public RawShellingPhase ShellingFirstRound { get; set; }
        [JsonProperty("api_hougeki2")]
        public RawShellingPhase ShellingSecondRound { get; set; }

        [JsonProperty("api_raigeki")]
        public RawTorpedoSalvoPhase ClosingTorpedoSalvo { get; set; }
    }
}

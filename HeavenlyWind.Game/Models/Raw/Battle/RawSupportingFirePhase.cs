using Newtonsoft.Json;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.Raw.Battle
{
    public class RawSupportingFirePhase
    {
        [JsonProperty("api_support_airatack")]
        public RawsupportAerialCombat AerialCombat { get; set; }

        [JsonProperty("api_support_hourai")]
        public RawSupportShelling Shelling { get; set; }

        public class RawsupportAerialCombat
        {
            [JsonProperty("api_deck_id")]
            public int FleetID { get; set; }
            [JsonProperty("api_ship_id")]
            public int[] ShipIDs { get; set; }

            [JsonProperty("api_stage1")]
            public RawAerialCombatPhase.RawStage1 Stage1 { get; set; }

            [JsonProperty("api_stage2")]
            public RawAerialCombatPhase.RawStage2 Stage2 { get; set; }

            [JsonProperty("api_stage3")]
            public RawAerialCombatPhase.RawStage3 Stage3 { get; set; }
        }

        public class RawSupportShelling
        {
            [JsonProperty("api_deck_id")]
            public int FleetID { get; set; }
            [JsonProperty("api_ship_id")]
            public int[] ShipIDs { get; set; }

            [JsonProperty("api_damage")]
            public int[] Damage { get; set; }
        }

    }
}

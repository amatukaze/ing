using Newtonsoft.Json;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.Raw.Battle
{
    public class RawSupportingFirePhase
    {
        [JsonProperty("api_support_hourai")]
        public RawSupportShelling Shelling { get; set; }

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

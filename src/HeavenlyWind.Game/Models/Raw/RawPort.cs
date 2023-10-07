using Newtonsoft.Json;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.Raw
{
    public class RawPort
    {
        [JsonProperty("api_material")]
        public RawMaterial[] Materials { get; set; }

        [JsonProperty("api_deck_port")]
        public RawFleet[] Fleets { get; set; }

        [JsonProperty("api_ndock")]
        public RawRepairDock[] RepairDocks { get; set; }

        [JsonProperty("api_ship")]
        public RawShip[] Ships { get; set; }

        [JsonProperty("api_basic")]
        public RawBasic Basic { get; set; }

        [JsonProperty("api_combined_flag")]
        public CombinedFleetType CombinedFleetType { get; set; }

        [JsonProperty("api_p_bgm_id")]
        public int BgmID { get; set; }

        [JsonProperty("api_event_object")]
        public RawEventData EventData { get; set; }

        public class RawEventData
        {
            [JsonProperty("api_m_flag2")]
            public int Flag { get; set; }
        }
    }
}

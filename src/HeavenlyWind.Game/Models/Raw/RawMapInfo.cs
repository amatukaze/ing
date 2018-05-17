using Newtonsoft.Json;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.Raw
{
    public class RawMapInfo : IID
    {
        [JsonProperty("api_id")]
        public int ID { get; set; }

        [JsonProperty("api_cleared")]
        public bool IsCleared { get; set; }

        [JsonProperty("api_eventmap")]
        public RawEventMap Event { get; set; }

        [JsonProperty("api_exboss_flag")]
        public bool IsIncompleted { get; set; }
        [JsonProperty("api_defeat_count")]
        public int? DefeatedCount { get; set; }

        [JsonProperty("api_air_base_decks")]
        public int AvailableAirBaseGroupCount { get; set; }

        public class RawEventMap
        {
            [JsonProperty("api_now_maphp")]
            public int HPCurrent { get; set; }
            [JsonProperty("api_max_maphp")]
            public int HPMaximum { get; set; }

            [JsonProperty("api_state")]
            public int State { get; set; }

            [JsonProperty("api_selected_rank")]
            public EventMapDifficulty SelectedDifficulty { get; set; }

            [JsonProperty("api_gauge_type")]
            public MapGaugeType? GaugeType { get; set; }
        }
    }
}

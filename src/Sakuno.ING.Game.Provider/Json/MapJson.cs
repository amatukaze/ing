using Newtonsoft.Json;
using Sakuno.ING.Game.Models;

namespace Sakuno.ING.Game.Json
{
    internal class MapJson : IRawMap
    {
        [JsonProperty("api_id")]
        public int Id { get; set; }
        [JsonProperty("api_cleared")]
        public bool IsCleared { get; set; }
        [JsonProperty("api_air_base_decks")]
        public int AvailableAirForceGroups { get; set; }
        [JsonProperty("api_defeat_count")]
        public int? DefeatedCount { get; set; }

        public EventMapJson api_eventmap;
        EventMapRank? IRawMap.Rank => api_eventmap?.api_selected_rank;
        EventMapGaugeType? IRawMap.GaugeType => api_eventmap?.api_gauge_type;
        int? IRawMap.GaugeIndex => api_eventmap?.api_gauge_num;
        ClampedValue? IRawMap.Gauge => api_eventmap != null ?
            (api_eventmap.api_now_maphp, api_eventmap.api_max_maphp)
            : (ClampedValue?)null;
    }

    internal class EventMapJson
    {
        public int api_now_maphp;
        public int api_max_maphp;
        public EventMapRank api_selected_rank;
        public EventMapGaugeType api_gauge_type;
        public int api_gauge_num;
    }
}

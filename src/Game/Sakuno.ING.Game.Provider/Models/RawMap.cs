using Newtonsoft.Json;
using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Models
{
    public sealed class RawMap : IIdentifiable<MapId>
    {
        internal RawMap() { }

        [JsonProperty("api_id")]
        public MapId Id { get; internal set; }
        [JsonProperty("api_cleared")]
        public bool IsCleared { get; internal set; }
        [JsonProperty("api_air_base_decks")]
        public int AvailableAirForceGroups { get; internal set; }
        [JsonProperty("api_defeat_count")]
        public int? DefeatedCount { get; internal set; }

        internal class EventMap
        {
            public int api_now_maphp;
            public int api_max_maphp;
            public EventMapRank api_selected_rank;
            public EventMapGaugeType api_gauge_type;
            public int api_gauge_num;
        }
        internal EventMap api_eventmap;
        public EventMapRank? Rank => api_eventmap?.api_selected_rank;
        public EventMapGaugeType? GaugeType => api_eventmap?.api_gauge_type;
        public int? GaugeIndex => api_eventmap?.api_gauge_num;
        public ClampedValue? Gauge => api_eventmap != null ?
            (api_eventmap.api_now_maphp, api_eventmap.api_max_maphp)
            : (ClampedValue?)null;
    }

    public enum EventMapRank
    {
        None = 0,
        /// <summary>
        /// 丁
        /// </summary>
        Casual = 1,
        /// <summary>
        /// 丙
        /// </summary>
        Easy = 2,
        /// <summary>
        /// 乙
        /// </summary>
        Normal = 3,
        /// <summary>
        /// 甲
        /// </summary>
        Hard = 4
    }

    public enum EventMapGaugeType
    {
        HP = 2,
        Transport = 3
    }
}

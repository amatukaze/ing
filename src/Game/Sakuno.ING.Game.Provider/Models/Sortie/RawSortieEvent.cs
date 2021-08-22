using Sakuno.ING.Game.Models.MasterData;
using System.Text.Json.Serialization;

namespace Sakuno.ING.Game.Models.Sortie
{
#nullable disable
    public sealed class RawSortieEvent
    {
        public int api_maparea_id { get; set; }
        public int api_mapinfo_no { get; set; }
        public MapId MapId => (MapId)(api_maparea_id * 10 + api_mapinfo_no);

        [JsonPropertyName("api_no")]
        public int RouteId { get; set; }

        [JsonPropertyName("api_event_id")]
        public SortieEventType Type { get; set; }
        [JsonPropertyName("api_event_kind")]
        public BattleType BattleType { get; set; }

        [JsonPropertyName("api_next")]
        public bool CanAdvance { get; set; }

        public bool api_m1 { get; set; }
    }
#nullable enable
}

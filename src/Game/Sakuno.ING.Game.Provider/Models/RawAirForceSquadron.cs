using System.Text.Json.Serialization;

namespace Sakuno.ING.Game.Models
{
    public sealed class RawAirForceSquadron : IIdentifiable
    {
        [JsonPropertyName("api_squadron_id")]
        public int Id { get; set; }

        [JsonPropertyName("api_slotid")]
        public SlotItemId? SlotItemId { get; set; }

        public int api_count { get; set; }
        public int api_max_count { get; set; }
        public ClampedValue Count => (api_count, api_max_count);

        [JsonPropertyName("api_cond")]
        public AirForceSquadronMorale Morale { get; set; }
    }
}

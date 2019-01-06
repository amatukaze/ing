using Newtonsoft.Json;

namespace Sakuno.ING.Game.Models
{
    public sealed class RawAirForceSquadron : IIdentifiable
    {
        internal RawAirForceSquadron() { }

        [JsonProperty("api_squadron_id")]
        public int Id { get; internal set; }
        [JsonProperty("api_slotid")]
        public EquipmentId? EquipmentId { get; internal set; }

        internal int api_count;
        internal int api_max_count;
        public ClampedValue AircraftCount => (api_count, api_max_count);

        [JsonProperty("api_cond")]
        public SquadronMorale Morale { get; internal set; }
    }

    public enum SquadronMorale
    {
        Normal = 1,
        Yellow = 2,
        Red = 3
    }
}

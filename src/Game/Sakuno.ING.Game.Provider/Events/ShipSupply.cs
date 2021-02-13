using Sakuno.ING.Game.Models;
using System.Text.Json.Serialization;

namespace Sakuno.ING.Game.Events
{
#nullable disable
    public sealed class ShipSupply
    {
        [JsonPropertyName("api_id")]
        public ShipId Id { get; set; }

        [JsonPropertyName("api_fuel")]
        public int CurrentFuel { get; set; }
        [JsonPropertyName("api_bull")]
        public int CurrentBullet { get; set; }
    }
#nullable enable
}

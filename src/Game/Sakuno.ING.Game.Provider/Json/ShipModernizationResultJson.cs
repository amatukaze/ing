using Sakuno.ING.Game.Json.Converters;
using Sakuno.ING.Game.Models;
using System.Text.Json.Serialization;

namespace Sakuno.ING.Game.Json
{
#nullable disable
    internal sealed class ShipModernizationResultJson
    {
        [JsonConverter(typeof(IntToBooleanConverter))]
        public bool api_powerup_flag { get; set; }
        public RawShip api_ship { get; set; }
        public RawFleet[] api_deck { get; set; }
    }
#nullable enable
}

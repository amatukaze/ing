using System.Collections.Generic;
using Newtonsoft.Json;
using Sakuno.ING.Game.Models;

namespace Sakuno.ING.Game.Events
{
    public sealed class ShipSupply
    {
        internal ShipSupply() { }

        [JsonProperty("api_id")]
        public ShipId ShipId { get; internal set; }
        [JsonProperty("api_fuel")]
        public int CurrentFuel { get; internal set; }
        [JsonProperty("api_bull")]
        public int CurrentBullet { get; internal set; }
        [JsonProperty("api_onslot")]
        public IReadOnlyList<int> SlotAircraft { get; internal set; }
    }
}

using System.Collections.Generic;
using Newtonsoft.Json;

namespace Sakuno.ING.Game.Models.MasterData
{
    public sealed class RawShipEquipLimitation
    {
        internal RawShipEquipLimitation() { }

        [JsonProperty("api_ship_id")]
        public ShipId Id { get; internal set; }

        [JsonProperty("api_equip_type")]
        public IReadOnlyCollection<EquipmentTypeId> AvailableEquipmentTypes { get; internal set; }
    }
}

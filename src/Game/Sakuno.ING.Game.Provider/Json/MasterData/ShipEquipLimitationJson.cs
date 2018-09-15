using System.Collections.Generic;
using Newtonsoft.Json;
using Sakuno.ING.Game.Models;
using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Json.MasterData
{
    class ShipEquipLimitationJson : IRawShipEquipLimitation
    {
        [JsonProperty("api_ship_id")]
        public ShipId Id { get; set; }

        [JsonProperty("api_equip_type")]
        public IReadOnlyCollection<EquipmentTypeId> AvailableEquipmentTypes { get; set; }
    }
}

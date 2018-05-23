using System.Collections.Generic;
using Newtonsoft.Json;
using Sakuno.ING.Game.Json.Converters;
using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Json.MasterData
{
    internal class ShipTypeInfoJson : IRawShipTypeInfo
    {
        [JsonProperty("api_id")]
        public ShipTypeId Id { get; set; }
        [JsonProperty("api_sortno")]
        public int SortNo { get; set; }
        [JsonProperty("api_name")]
        public string Name { get; set; }

        [JsonProperty("api_scnt")]
        public int RepairTimeRatio { get; set; }
        [JsonProperty("api_kcnt")]
        public int BuildOutlineId { get; set; }

        [JsonProperty("api_equip_type"), JsonConverter(typeof(BoolDictionaryConverter))]
        public IReadOnlyCollection<EquipmentTypeId> AvailableEquipmentTypes { get; set; }
    }
}

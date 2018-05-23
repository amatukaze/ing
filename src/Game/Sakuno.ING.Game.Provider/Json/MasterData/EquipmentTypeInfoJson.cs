using Newtonsoft.Json;
using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Json.MasterData
{
    internal class EquipmentTypeInfoJson : IRawEquipmentTypeInfo
    {
        [JsonProperty("api_id")]
        public EquipmentTypeId Id { get; set; }
        [JsonProperty("api_name")]
        public string Name { get; set; }

        public bool AvailableInExtraSlot { get; set; }
    }
}

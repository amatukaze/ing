using Newtonsoft.Json;
using Sakuno.KanColle.Amatsukaze.Game.Models.MasterData;

namespace Sakuno.KanColle.Amatsukaze.Game.Json.MasterData
{
    internal class EquipmentTypeInfoJson : IRawEquipmentTypeInfo
    {
        [JsonProperty("api_id")]
        public int Id { get; set; }
        [JsonProperty("api_name")]
        public string Name { get; set; }

        public bool AvailableInExtraSlot { get; set; }
    }
}

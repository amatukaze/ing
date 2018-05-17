using Newtonsoft.Json;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.Raw
{
    public class RawEquipmentTypeInfo : IID
    {
        [JsonProperty("api_id")]
        public int ID { get; set; }

        [JsonProperty("api_name")]
        public string Name { get; set; }
    }
}

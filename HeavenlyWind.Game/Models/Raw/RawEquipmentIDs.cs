using Newtonsoft.Json;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.Raw
{
    public class RawEquipmentIDs
    {
        [JsonProperty("api_slot")]
        public int[] EquipmentIDs { get; set; }
    }
}

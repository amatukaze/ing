using Newtonsoft.Json;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.Raw
{
    public class RawEquipmentSetup
    {
        [JsonProperty("api_bauxite")]
        public int? Bauxite { get; set; }
    }
}

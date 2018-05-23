using Newtonsoft.Json;
using Sakuno.ING.Game.Json.Converters;
using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Json.MasterData
{
    internal class FurnitureInfoJson : IRawFurnitureInfo
    {
        [JsonProperty("api_id")]
        public int Id { get; set; }
        [JsonProperty("api_type")]
        public int Type { get; set; }
        [JsonProperty("api_no")]
        public int CategoryNo { get; set; }
        [JsonProperty("api_title")]
        public string Name { get; set; }
        [JsonProperty("api_description"), JsonConverter(typeof(HtmlNewLineEater))]
        public string Description { get; set; }

        [JsonProperty("api_rarity")]
        public int Rarity { get; set; }
        [JsonProperty("api_price")]
        public int Price { get; set; }
        [JsonProperty("api_saleflg")]
        public bool IsSale { get; set; }
    }
}

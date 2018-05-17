using Newtonsoft.Json;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.Raw
{
    public class RawShipInfo : IID
    {
        [JsonProperty("api_id")]
        public int ID { get; set; }

        [JsonProperty("api_sortno")]
        public int SortNumber { get; set; }

        [JsonProperty("api_name")]
        public string Name { get; set; }
        [JsonProperty("api_yomi")]
        public string NameReading { get; set; }

        [JsonProperty("api_stype")]
        public int Type { get; set; }

        [JsonProperty("api_afterlv")]
        public int RemodelingMinimumLevel { get; set; }
        [JsonProperty("api_aftershipid")]
        public int ShipIDAfterRemodel { get; set; }

        [JsonProperty("api_taik")]
        public int[] HP { get; set; }
        [JsonProperty("api_souk")]
        public int[] Armor { get; set; }
        [JsonProperty("api_houg")]
        public int[] Firepower { get; set; }
        [JsonProperty("api_raig")]
        public int[] Torpedo { get; set; }
        [JsonProperty("api_tyku")]
        public int[] AA { get; set; }
        [JsonProperty("api_luck")]
        public int[] Luck { get; set; }

        [JsonProperty("api_soku")]
        public ShipSpeed Speed { get; set; }

        [JsonProperty("api_leng")]
        public int Range { get; set; }

        [JsonProperty("api_slot_num")]
        public int SlotCount { get; set; }

        [JsonProperty("api_maxeq")]
        public int[] PlaneCountInSlot { get; set; }

        [JsonProperty("api_buildtime")]
        public int ConstructionTime { get; set; }

        //[JsonProperty("api_broken")]
        //public int[] ApiBroken { get; set; }

        //[JsonProperty("api_powup")]
        //public int[] ApiPowup { get; set; }

        [JsonProperty("api_backs")]
        public int Rarity { get; set; }

        //[JsonProperty("api_getmes")]
        //public string ApiGetmes { get; set; }

        [JsonProperty("api_afterfuel")]
        public int RemodelingFuelConsumption { get; set; }
        [JsonProperty("api_afterbull")]
        public int RemodelingBulletConsumption { get; set; }

        [JsonProperty("api_fuel_max")]
        public int MaxFuelConsumption { get; set; }
        [JsonProperty("api_bull_max")]
        public int MaxBulletConsumption { get; set; }

        //[JsonProperty("api_voicef")]
        //public int ApiVoicef { get; set; }
    }
}

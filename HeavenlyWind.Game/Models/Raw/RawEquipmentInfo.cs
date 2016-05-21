using Newtonsoft.Json;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.Raw
{
    public class RawEquipmentInfo : IID
    {
        [JsonProperty("api_id")]
        public int ID { get; set; }

        [JsonProperty("api_sortno")]
        public int SortNumber { get; set; }

        [JsonProperty("api_name")]
        public string Name { get; set; }

        [JsonProperty("api_type")]
        public int[] Type { get; set; }

        [JsonProperty("api_taik")]
        public int HP { get; set; }
        [JsonProperty("api_souk")]
        public int Armor { get; set; }
        [JsonProperty("api_houg")]
        public int Firepower { get; set; }
        [JsonProperty("api_raig")]
        public int Torpedo { get; set; }
        [JsonProperty("api_soku")]
        public int Speed { get; set; }
        [JsonProperty("api_baku")]
        public int DiveBomberAttack { get; set; }
        [JsonProperty("api_tyku")]
        public int AA { get; set; }
        [JsonProperty("api_tais")]
        public int ASW { get; set; }
        //[JsonProperty("api_atap")]
        //public int ApiAtap { get; set; }
        [JsonProperty("api_houm")]
        public int Accuracy { get; set; }
        //[JsonProperty("api_raim")]
        //public int ApiRaim { get; set; }
        [JsonProperty("api_houk")]
        public int Evasion { get; set; }
        //[JsonProperty("api_raik")]
        //public int ApiRaik { get; set; }
        //[JsonProperty("api_bakk")]
        //public int ApiBakk { get; set; }
        [JsonProperty("api_saku")]
        public int LoS { get; set; }
        //[JsonProperty("api_sakb")]
        //public int ApiSakb { get; set; }
        [JsonProperty("api_luck")]
        public int Luck { get; set; }
        [JsonProperty("api_leng")]
        public int Range { get; set; }

        [JsonProperty("api_rare")]
        public int Rarity { get; set; }

        //[JsonProperty("api_broken")]
        //public int[] ApiBroken { get; set; }

        [JsonProperty("api_info")]
        public string Information { get; set; }

        //[JsonProperty("api_usebull")]
        //public string UseBullet { get; set; }

        [JsonProperty("api_distance")]
        public int CombatRadius { get; set; }
    }
}

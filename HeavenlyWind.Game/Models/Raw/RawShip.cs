using Newtonsoft.Json;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.Raw
{
    public class RawShip : IID
    {
        [JsonProperty("api_id")]
        public int ID { get; set; }

        [JsonProperty("api_sortno")]
        public int SortNumber { get; set; }

        [JsonProperty("api_ship_id")]
        public int ShipID { get; set; }

        [JsonProperty("api_lv")]
        public int Level { get; set; }

        [JsonProperty("api_exp")]
        public int[] Experience { get; set; }

        [JsonProperty("api_nowhp")]
        public int HPCurrent { get; set; }
        [JsonProperty("api_maxhp")]
        public int HPMaximum { get; set; }

        [JsonProperty("api_leng")]
        public int Range { get; set; }

        [JsonProperty("api_slot")]
        public int[] Equipments { get; set; }

        [JsonProperty("api_onslot")]
        public int[] PlaneCountInSlot { get; set; }
        [JsonProperty("api_slot_ex")]
        public int ExtraEquipment { get; set; }

        [JsonProperty("api_kyouka")]
        public int[] ModernizedStatus { get; set; }

        [JsonProperty("api_backs")]
        public int Rarity { get; set; }

        [JsonProperty("api_fuel")]
        public int Fuel { get; set; }
        [JsonProperty("api_bull")]
        public int Bullet { get; set; }

        [JsonProperty("api_slotnum")]
        public int EquipmentCount { get; set; }

        [JsonProperty("api_ndock_time")]
        public int RepairTime { get; set; }
        [JsonProperty("api_ndock_item")]
        public int[] RepairConsumption { get; set; }

        //[JsonProperty("api_srate")]
        //public int ApiSrate { get; set; }

        [JsonProperty("api_cond")]
        public int Condition { get; set; }

        [JsonProperty("api_karyoku")]
        public int[] Firepower { get; set; }
        [JsonProperty("api_raisou")]
        public int[] Torpedo { get; set; }
        [JsonProperty("api_taiku")]
        public int[] AA { get; set; }
        [JsonProperty("api_soukou")]
        public int[] Armor { get; set; }
        [JsonProperty("api_kaihi")]
        public int[] Evasion { get; set; }
        [JsonProperty("api_taisen")]
        public int[] ASW { get; set; }
        [JsonProperty("api_sakuteki")]
        public int[] LoS { get; set; }
        [JsonProperty("api_lucky")]
        public int[] Luck { get; set; }

        [JsonProperty("api_locked")]
        public bool IsLocked { get; set; }
        [JsonProperty("api_locked_equip")]
        public bool HasLockedEquipment { get; set; }

        [JsonProperty("api_sally_area")]
        public int LockingTag { get; set; }
    }
}

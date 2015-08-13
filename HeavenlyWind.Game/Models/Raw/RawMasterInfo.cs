using Newtonsoft.Json;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.Raw
{
    public class RawMasterInfo
    {
        [JsonProperty("api_mst_ship")]
        public RawShipInfo[] Ships { get; set; }

        [JsonProperty("api_mst_slotitem_equiptype")]
        public RawEquipmentTypeInfo[] EquipmentTypes { get; set; }

        [JsonProperty("api_mst_stype")]
        public RawShipType[] ShipTypes { get; set; }

        [JsonProperty("api_mst_slotitem")]
        public RawEquipmentInfo[] Equipments { get; set; }
    }
}

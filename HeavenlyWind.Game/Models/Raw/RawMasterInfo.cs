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
        public RawEquipmentInfo[] Equipment { get; set; }

        [JsonProperty("api_mst_useitem")]
        public RawItemInfo[] Items { get; set; }

        [JsonProperty("api_mst_maparea")]
        public RawMapAreaInfo[] MapAreas { get; set; }
        [JsonProperty("api_mst_mapinfo")]
        public RawMapMasterInfo[] Maps { get; set; }

        [JsonProperty("api_mst_mission")]
        public RawExpeditionInfo[] Expeditions { get; set; }
    }
}

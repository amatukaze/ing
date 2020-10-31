using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Json
{
#nullable disable
    internal sealed class MasterDataJson
    {
        public RawShipInfo[] api_mst_ship { get; set; }
        public RawSlotItemTypeInfo[] api_mst_slotitem_equiptype { get; set; }
        public SlotItemInfoId[] api_mst_equip_exslot { get; set; }
        public RawExtraSlotSlotItemAllowListJson[] api_mst_equip_exslot_ship { get; set; }
        public RawShipTypeInfo[] api_mst_stype { get; set; }
        public RawSlotItemInfo[] api_mst_slotitem { get; set; }
        public RawUseItem[] api_mst_useitem { get; set; }
        public RawMapArea[] api_mst_maparea { get; set; }
        public RawMapInfo[] api_mst_mapinfo { get; set; }
        public RawMapBgmInfo[] api_mst_mapbgm { get; set; }
        public RawExpeditionInfo[] api_mst_mission { get; set; }
        public RawShipEquipLimitation[] api_mst_equip_ship { get; set; }
    }
#nullable enable
}

using System.Collections.Generic;
using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Json.MasterData
{
    internal class MasterDataJson
    {
        public RawShipInfo[] api_mst_ship;
        public List<EquipmentInfoId> api_mst_equip_exslot;
        public EquipmentInExtraSlotJson[] api_mst_equip_exslot_ship;
        public RawShipTypeInfo[] api_mst_stype;
        public List<RawEquipmentTypeInfo> api_mst_slotitem_equiptype;
        public List<RawEquipmentInfo> api_mst_slotitem;
        public RawFurnitureInfo[] api_mst_furniture;
        public RawUseItem[] api_mst_useitem;
        public RawMapArea[] api_mst_maparea;
        public RawMapInfo[] api_mst_mapinfo;
        public List<RawMapBgmInfo> api_mst_mapbgm;
        public RawExpeditionInfo[] api_mst_mission;
        public List<ShipUpgradeJson> api_mst_shipupgrade;
        public RawBgmInfo[] api_mst_bgm;
        public RawShipEquipLimitation[] api_mst_equip_ship;
    }
    internal class EquipmentInExtraSlotJson
    {
        public EquipmentInfoId api_slotitem_id;
        public List<ShipInfoId> api_ship_ids;
    }
    internal class ShipUpgradeJson
    {
        public int api_id;
        public ShipInfoId? api_current_ship_id;
        public ShipInfoId api_original_ship_id;
        public int api_drawing_count;
        public int api_catapult_count;
        public int api_report_count;
    }
}

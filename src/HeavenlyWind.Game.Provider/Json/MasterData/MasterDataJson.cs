namespace Sakuno.KanColle.Amatsukaze.Game.Json.MasterData
{
    internal class MasterDataJson
    {
        public ShipInfoJson[] api_mst_ship;
        public int[] api_mst_equip_exslot;
        public EquipmentInExtraSlotJson[] api_mst_equip_exslot_ship;
        public ShipTypeInfoJson[] api_mst_stype;
        public EquipmentTypeInfoJson[] api_mst_slotitem_equiptype;
        public EquipmentInfoJson[] api_mst_slotitem;
        public FurnitureInfoJson[] api_mst_furniture;
        public UseItemJson[] api_mst_useitem;
        public MapAreaJson[] api_mst_maparea;
        public MapInfoJson[] api_mst_mapinfo;
        public MapBgmInfoJson[] api_mst_mapbgm;
        public ExpeditionInfoJson[] api_mst_mission;
        public ShipUpgradeJson[] api_mst_shipupgrade;
        public BgmInfoJson[] api_mst_bgm;
    }
    internal class EquipmentInExtraSlotJson
    {
        public int api_slotitem_id;
        public int[] api_ship_ids;
    }
    internal class ShipUpgradeJson
    {
        public int api_id;
        public int api_current_ship_id;
        public int api_original_ship_id;
        public int api_drawing_count;
        public int api_catapult_count;
        public int api_report_count;
    }
}

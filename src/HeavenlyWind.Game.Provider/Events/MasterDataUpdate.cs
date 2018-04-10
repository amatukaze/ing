using Sakuno.KanColle.Amatsukaze.Game.Models.MasterData;

namespace Sakuno.KanColle.Amatsukaze.Game.Events
{
    public class MasterDataUpdate
    {
        public EquipmentInfo[] Equipments;
        public EquipmentTypeInfo[] EquipmentTypes;
        public ExpeditionInfo[] Expeditions;
        public ItemInfo[] Items;
        public MapAreaInfo[] MapAreas;
        public MapInfo[] Maps;
        public ShipInfo[] Ships;
        public ShipTypeInfo[] ShipTypes;
    }
}

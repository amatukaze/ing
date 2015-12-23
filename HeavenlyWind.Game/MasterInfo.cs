using Sakuno.KanColle.Amatsukaze.Game.Models;
using Sakuno.KanColle.Amatsukaze.Game.Models.Raw;

namespace Sakuno.KanColle.Amatsukaze.Game
{
    public class MasterInfo
    {
        public IDTable<ShipInfo> Ships { get; } = new IDTable<ShipInfo>();
        public IDTable<ShipType> ShipTypes { get; } = new IDTable<ShipType>();

        public IDTable<EquipmentInfo> Equipments { get; } = new IDTable<EquipmentInfo>();
        public IDTable<EquipmentTypeInfo> EquipmentTypes { get; } = new IDTable<EquipmentTypeInfo>();

        public IDTable<ItemInfo> Items { get; } = new IDTable<ItemInfo>();

        public IDTable<MapAreaInfo> MapAreas { get; } = new IDTable<MapAreaInfo>();
        public IDTable<MapMasterInfo> Maps { get; } = new IDTable<MapMasterInfo>();

        public IDTable<ExpeditionInfo> Expeditions { get; } = new IDTable<ExpeditionInfo>();

        internal MasterInfo() { }

        public void Update(RawMasterInfo rpInfo)
        {
            Ships.UpdateRawData(rpInfo.Ships, r => new ShipInfo(r), (rpData, rpRawData) => rpData.Update(rpRawData));
            ShipTypes.UpdateRawData(rpInfo.ShipTypes, r => new ShipType(r), (rpData, rpRawData) => rpData.Update(rpRawData));

            Equipments.UpdateRawData(rpInfo.Equipments, r => new EquipmentInfo(r), (rpData, rpRawData) => rpData.Update(rpRawData));
            EquipmentTypes.UpdateRawData(rpInfo.EquipmentTypes, r => new EquipmentTypeInfo(r), (rpData, rpRawData) => rpData.Update(rpRawData));

            Items.UpdateRawData(rpInfo.Items, r => new ItemInfo(r), (rpData, rpRawData) => rpData.Update(rpRawData));

            MapAreas.UpdateRawData(rpInfo.MapAreas, r => new MapAreaInfo(r), (rpData, rpRawData) => rpData.Update(rpRawData));
            Maps.UpdateRawData(rpInfo.Maps, r => new MapMasterInfo(r), (rpData, rpRawData) => rpData.Update(rpRawData));

            Expeditions.UpdateRawData(rpInfo.Expeditions, r => new ExpeditionInfo(r), (rpData, rpRawData) => rpData.Update(rpRawData));
        }
    }
}

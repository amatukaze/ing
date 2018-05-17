using Sakuno.KanColle.Amatsukaze.Game.Models;
using Sakuno.KanColle.Amatsukaze.Game.Models.Raw;
using System.Linq;
using System.Threading;

namespace Sakuno.KanColle.Amatsukaze.Game
{
    public class MasterInfo : ModelBase
    {
        ManualResetEventSlim r_InitializationLock = new ManualResetEventSlim(false);

        public IDTable<ShipInfo> Ships { get; } = new IDTable<ShipInfo>();
        public IDTable<ShipTypeInfo> ShipTypes { get; } = new IDTable<ShipTypeInfo>();

        public IDTable<EquipmentInfo> Equipment { get; } = new IDTable<EquipmentInfo>();
        public IDTable<EquipmentTypeInfo> EquipmentTypes { get; } = new IDTable<EquipmentTypeInfo>();

        public IDTable<FurnitureInfo> Furnitures { get; } = new IDTable<FurnitureInfo>();
        public IDTable<ItemInfo> Items { get; } = new IDTable<ItemInfo>();

        public IDTable<MapAreaInfo> MapAreas { get; } = new IDTable<MapAreaInfo>();
        public IDTable<MapMasterInfo> Maps { get; } = new IDTable<MapMasterInfo>();

        public IDTable<ExpeditionInfo> Expeditions { get; } = new IDTable<ExpeditionInfo>();

        public int EventMapCount { get; private set; }

        internal MasterInfo() { }

        public void Update(RawMasterInfo rpInfo)
        {
            Ships.UpdateRawData(rpInfo.Ships, r => new ShipInfo(r), (rpData, rpRawData) => rpData.Update(rpRawData));
            ShipTypes.UpdateRawData(rpInfo.ShipTypes, r => new ShipTypeInfo(r), (rpData, rpRawData) => rpData.Update(rpRawData));

            Equipment.UpdateRawData(rpInfo.Equipment, r => new EquipmentInfo(r), (rpData, rpRawData) => rpData.Update(rpRawData));
            EquipmentTypes.UpdateRawData(rpInfo.EquipmentTypes, r => new EquipmentTypeInfo(r), (rpData, rpRawData) => rpData.Update(rpRawData));

            Furnitures.UpdateRawData(rpInfo.Furnitures, r => new FurnitureInfo(r), (rpData, rpRawData) => rpData.Update(rpRawData));
            Items.UpdateRawData(rpInfo.Items, r => new ItemInfo(r), (rpData, rpRawData) => rpData.Update(rpRawData));

            MapAreas.UpdateRawData(rpInfo.MapAreas, r => new MapAreaInfo(r), (rpData, rpRawData) => rpData.Update(rpRawData));
            Maps.UpdateRawData(rpInfo.Maps, r => new MapMasterInfo(r), (rpData, rpRawData) => rpData.Update(rpRawData));

            Expeditions.UpdateRawData(rpInfo.Expeditions, r => new ExpeditionInfo(r), (rpData, rpRawData) => rpData.Update(rpRawData));

            EventMapCount = (from rArea in MapAreas.Values
                             where rArea.IsEventArea
                             join rMap in Maps.Values on rArea.ID equals rMap.AreaID
                             select rMap).Count();

            if (r_InitializationLock != null)
            {
                r_InitializationLock.Set();
                r_InitializationLock.Dispose();
                r_InitializationLock = null;
            }
        }

        public void WaitForInitialization() => r_InitializationLock?.Wait();

        public ExpeditionInfo GetExpeditionFromName(string rpName)
        {
            foreach (var rExpedition in Expeditions.Values)
                if (rExpedition.Name == rpName)
                    return rExpedition;

            return null;
        }
    }
}

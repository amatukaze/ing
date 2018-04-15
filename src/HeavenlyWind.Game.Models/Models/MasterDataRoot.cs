using Sakuno.KanColle.Amatsukaze.Game.Events;
using Sakuno.KanColle.Amatsukaze.Game.Models.MasterData;

namespace Sakuno.KanColle.Amatsukaze.Game.Models
{
    public class MasterDataRoot
    {
        internal MasterDataRoot(GameListener listener)
        {
            listener.MasterDataUpdated.Received += OnMasterDataUpdated;
        }

        private void OnMasterDataUpdated(MasterDataUpdate obj)
        {
            ShipTypes.BatchUpdate(obj.ShipTypes);
            ShipInfos.BatchUpdate(obj.ShipInfos);
            EquipmentTypes.BatchUpdate(obj.EquipmentTypes);
            EquipmentInfos.BatchUpdate(obj.EquipmentInfos);
            MapAreas.BatchUpdate(obj.MapAreas);
            MapInfos.BatchUpdate(obj.Maps);
            Expeditions.BatchUpdate(obj.Expeditions);
        }

        public IDTable<ShipInfo, IRawShipInfo> ShipInfos { get; } = new IDTable<ShipInfo, IRawShipInfo>();
        public IDTable<ShipTypeInfo, IRawShipTypeInfo> ShipTypes { get; } = new IDTable<ShipTypeInfo, IRawShipTypeInfo>();
        public IDTable<EquipmentTypeInfo, IRawEquipmentTypeInfo> EquipmentTypes { get; } = new IDTable<EquipmentTypeInfo, IRawEquipmentTypeInfo>();
        public IDTable<EquipmentInfo, IRawEquipmentInfo> EquipmentInfos { get; } = new IDTable<EquipmentInfo, IRawEquipmentInfo>();
        public IDTable<MapAreaInfo, IRawMapArea> MapAreas { get; } = new IDTable<MapAreaInfo, IRawMapArea>();
        public IDTable<MapInfo, IRawMapInfo> MapInfos { get; } = new IDTable<MapInfo, IRawMapInfo>();
        public IDTable<ExpeditionInfo, IRawExpeditionInfo> Expeditions { get; } = new IDTable<ExpeditionInfo, IRawExpeditionInfo>();
    }
}

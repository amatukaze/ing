using Sakuno.KanColle.Amatsukaze.Game.Models;
using Sakuno.KanColle.Amatsukaze.Game.Models.Raw;
using System.Collections.Generic;
using System.Linq;

namespace Sakuno.KanColle.Amatsukaze.Game
{
    public class Port : ModelBase
    {
        public Admiral Admiral { get; } = new Admiral(null);

        public Materials Materials { get; } = new Materials();

        public HashSet<int> ShipIDs { get; private set; }
        public IDTable<Ship> Ships { get; } = new IDTable<Ship>();

        public IDTable<Fleet> Fleets { get; } = new IDTable<Fleet>();

        public IDTable<Equipment> Equipments { get; } = new IDTable<Equipment>();

        public IDTable<RepairDock> RepairDocks { get; } = new IDTable<RepairDock>();
        public IDTable<BuildingDock> BuildingDocks { get; } = new IDTable<BuildingDock>();

        public IDTable<Quest> Quests { get; } = new IDTable<Quest>();

        internal Port()
        {
        }

        #region Update

        internal void UpdatePort(RawPort rpPort)
        {
            Admiral.Update(rpPort.Basic);
            Materials.Update(rpPort.Materials);

            if (Ships.UpdateRawData<RawShip>(rpPort.Ships, r => new Ship(r), (rpData, rpRawData) => rpData.Update(rpRawData)))
            {
                ShipIDs = new HashSet<int>(Ships.Values.Select(r => r.Info.ID));
                OnPropertyChanged(nameof(Ships));
            }
            
            RepairDocks.UpdateRawData<RawRepairDock>(rpPort.RepairDocks, r => new RepairDock(r), (rpData, rpRawData) => rpData.Update(rpRawData));

            Fleets.UpdateRawData<RawFleet>(rpPort.Fleets, r => new Fleet(this, r), (rpData, rpRawData) => rpData.Update(rpRawData));
        }

        internal void UpdateEquipments(RawEquipment[] rpEquipments)
        {
            if (Equipments.UpdateRawData<RawEquipment>(rpEquipments, r => new Equipment(r), (rpData, rpRawData) => rpData.Update(rpRawData)))
                OnPropertyChanged(nameof(Equipments));
        }

        internal void UpdateBuildingDocks(RawBuildingDock[] rpBuildingDocks)
        {
            BuildingDocks.UpdateRawData<RawBuildingDock>(rpBuildingDocks, r => new BuildingDock(r), (rpData, rpRawData) => rpData.Update(rpRawData));
        }

        #endregion
    }
}

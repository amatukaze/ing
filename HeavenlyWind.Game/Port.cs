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

        public FleetManager Fleets { get; } = new FleetManager();

        public IDTable<Equipment> Equipments { get; } = new IDTable<Equipment>();

        public IDTable<RepairDock> RepairDocks { get; } = new IDTable<RepairDock>();
        public IDTable<ConstructionDock> ConstructionDocks { get; } = new IDTable<ConstructionDock>();

        public QuestManager Quests { get; } = new QuestManager();

        internal Port()
        {
        }

        #region Update

        internal void UpdateAdmiral(RawBasic rpAdmiral)
        {
            Admiral.Update(rpAdmiral);
            OnPropertyChanged(nameof(Admiral));
        }

        internal void UpdatePort(RawPort rpPort)
        {
            UpdateAdmiral(rpPort.Basic);
            Materials.Update(rpPort.Materials);

            if (Ships.UpdateRawData<RawShip>(rpPort.Ships, r => new Ship(r), (rpData, rpRawData) => rpData.Update(rpRawData)))
            {
                ShipIDs = new HashSet<int>(Ships.Values.Select(r => r.Info.ID));
                OnPropertyChanged(nameof(Ships));
            }

            if (RepairDocks.UpdateRawData<RawRepairDock>(rpPort.RepairDocks, r => new RepairDock(r), (rpData, rpRawData) => rpData.Update(rpRawData)))
                OnPropertyChanged(nameof(RepairDocks));

            Fleets.Update(rpPort);
        }

        internal void UpdateEquipments(RawEquipment[] rpEquipments)
        {
            if (Equipments.UpdateRawData<RawEquipment>(rpEquipments, r => new Equipment(r), (rpData, rpRawData) => rpData.Update(rpRawData)))
                OnPropertyChanged(nameof(Equipments));
        }
        internal void AddEquipment(Equipment rpEquipment)
        {
            Equipments.Add(rpEquipment);
            OnPropertyChanged(nameof(Equipments));
        }

        internal void UpdateConstructionDocks(RawConstructionDock[] rpConstructionDocks)
        {
            if (ConstructionDocks.UpdateRawData<RawConstructionDock>(rpConstructionDocks, r => new ConstructionDock(r), (rpData, rpRawData) => rpData.Update(rpRawData)))
                OnPropertyChanged(nameof(ConstructionDocks));
        }

        #endregion
    }
}

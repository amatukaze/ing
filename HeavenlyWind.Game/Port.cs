using Sakuno.KanColle.Amatsukaze.Game.Models;
using Sakuno.KanColle.Amatsukaze.Game.Models.Raw;
using Sakuno.KanColle.Amatsukaze.Game.Services;
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
            SessionService.Instance.Subscribe("api_get_member/ship_deck", r =>
            {
                var rData = r.GetData<RawShipsAndFleets>();
                UpdateShips(rData.Ships);
                Fleets.Update(rData.Fleets);
            });
            SessionService.Instance.Subscribe("api_get_member/ship3", r =>
            {
                var rData = r.GetData<RawShipsAndFleets>();
                UpdateShips(rData.Ships);
                Fleets.Update(rData.Fleets);
            });
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

            UpdateShips(rpPort);

            if (RepairDocks.UpdateRawData(rpPort.RepairDocks, r => new RepairDock(r), (rpData, rpRawData) => rpData.Update(rpRawData)))
                OnPropertyChanged(nameof(RepairDocks));

            Fleets.Update(rpPort);
        }

        internal void UpdateShips(RawPort rpPort) => UpdateShips(rpPort.Ships);
        internal void UpdateShips(RawShip[] rpShips)
        {
            if (Ships.UpdateRawData(rpShips, r => new Ship(r), (rpData, rpRawData) => rpData.Update(rpRawData)))
                UpdateShipsCore();
        }

        internal void UpdateShipsCore()
        {
            ShipIDs = new HashSet<int>(Ships.Values.Select(r => r.Info.ID));
            OnPropertyChanged(nameof(Ships));
        }

        internal void UpdateEquipments(RawEquipment[] rpEquipments)
        {
            if (Equipments.UpdateRawData(rpEquipments, r => new Equipment(r), (rpData, rpRawData) => rpData.Update(rpRawData)))
                OnPropertyChanged(nameof(Equipments));
        }
        internal void AddEquipment(Equipment rpEquipment)
        {
            Equipments.Add(rpEquipment);
            OnPropertyChanged(nameof(Equipments));
        }

        internal void UpdateConstructionDocks(RawConstructionDock[] rpConstructionDocks)
        {
            if (ConstructionDocks.UpdateRawData(rpConstructionDocks, r => new ConstructionDock(r), (rpData, rpRawData) => rpData.Update(rpRawData)))
                OnPropertyChanged(nameof(ConstructionDocks));
        }

        #endregion
    }
}

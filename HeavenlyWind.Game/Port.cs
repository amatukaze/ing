using Sakuno.KanColle.Amatsukaze.Game.Models;
using Sakuno.KanColle.Amatsukaze.Game.Models.Raw;
using Sakuno.KanColle.Amatsukaze.Game.Parsers;
using Sakuno.KanColle.Amatsukaze.Game.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sakuno.KanColle.Amatsukaze.Game
{
    public class Port : ModelBase
    {
        Admiral r_Admiral;
        public Admiral Admiral
        {
            get { return r_Admiral; }
            private set
            {
                if (r_Admiral != value)
                {
                    r_Admiral = value;
                    OnPropertyChanged(nameof(Admiral));
                }
            }
        }

        public Materials Materials { get; } = new Materials();

        public HashSet<int> ShipIDs { get; private set; }
        public IDTable<Ship> Ships { get; } = new IDTable<Ship>();

        public FleetManager Fleets { get; } = new FleetManager();

        public IDTable<Equipment> Equipments { get; } = new IDTable<Equipment>();

        public IDTable<RepairDock> RepairDocks { get; } = new IDTable<RepairDock>();
        public IDTable<ConstructionDock> ConstructionDocks { get; } = new IDTable<ConstructionDock>();

        public QuestManager Quests { get; } = new QuestManager();

        int r_PendingShipCount;
        public int PendingShipCount
        {
            get { return r_PendingShipCount; }
            private set
            {
                if (r_PendingShipCount != value)
                {
                    r_PendingShipCount = value;
                    OnPropertyChanged(nameof(PendingShipCount));
                }
            }
        }

        internal Port()
        {
            SessionService.Instance.Subscribe("api_get_member/ship_deck", r =>
            {
                var rData = r.GetData<RawShipsAndFleets>();

                foreach (var rRawShip in rData.Ships)
                    Ships[rRawShip.ID].Update(rRawShip);

                var rRawFleet = rData.Fleets[0];
                Fleets[rRawFleet.ID].Update(rRawFleet);

                OnPropertyChanged(nameof(Ships));
            });
            SessionService.Instance.Subscribe("api_get_member/ship3", r =>
            {
                var rData = r.GetData<RawShipsAndFleets>();
                foreach (var rShip in rData.Ships)
                    Ships[rShip.ID].Update(rShip);
                Fleets.Update(rData.Fleets);
            });

            SessionService.Instance.Subscribe("api_req_kousyou/destroyship", r =>
            {
                var rShip = Ships[int.Parse(r.Requests["api_ship_id"])];

                rShip.OwnerFleet?.Remove(rShip);
                Ships.Remove(rShip);
                UpdateShipsCore();

            });
            SessionService.Instance.Subscribe("api_req_kousyou/destroyitem2", r =>
            {
                var rEquipmentIDs = r.Requests["api_slotitem_ids"].Split(',').Select(int.Parse);

                foreach (var rEquipmentID in rEquipmentIDs)
                    Equipments.Remove(rEquipmentID);

                OnPropertyChanged(nameof(Equipments));
            });

            SessionService.Instance.Subscribe("api_req_hokyu/charge", r =>
            {
                var rData = r.GetData<RawSupplyResult>();
                var rFleets = new HashSet<Fleet>();

                foreach (var rShipSupplyResult in rData.Ships)
                {
                    var rShip = Ships[rShipSupplyResult.ID];
                    rShip.Fuel = rShip.Fuel.Update(rShipSupplyResult.Fuel);
                    rShip.Bullet = rShip.Bullet.Update(rShipSupplyResult.Bullet);

                    rFleets.Add(rShip.OwnerFleet);

                    var rPlanes = rShipSupplyResult.Planes;
                    for (var i = 0; i < rShip.Slots.Count; i++)
                    {
                        var rCount = rPlanes[i];
                        if (rCount > 0)
                            rShip.Slots[i].PlaneCount = rCount;
                    }
                }

                foreach (var rFleet in rFleets)
                    rFleet.Update();
            });

            SessionService.Instance.Subscribe("api_get_member/ndock", r => UpdateRepairDocks(r.GetData<RawRepairDock[]>()));
            SessionService.Instance.Subscribe("api_req_nyukyo/start", r =>
            {
                var rShip = Ships[int.Parse(r.Requests["api_ship_id"])];
                var rIsInstantRepair = r.Requests["api_highspeed"] == "1";
                rShip.Repair(rIsInstantRepair);
                rShip.OwnerFleet?.Update();
            });

            Action<ApiData> rProcessIfShipDropped = r =>
            {
                var rData = (RawBattleResult)r.Data;
                if (rData.DroppedShip != null)
                {
                    PendingShipCount++;

                    Logger.Write(LoggingLevel.Info, string.Format(StringResources.Instance.Main.Log_Ship_Dropped, rData.DroppedShip.Name));
                }
            };
            SessionService.Instance.Subscribe("api_req_sortie/battleresult", rProcessIfShipDropped);
            SessionService.Instance.Subscribe("api_req_combined_battle/battleresult", rProcessIfShipDropped);
            SessionService.Instance.Subscribe("api_port/port", _ => PendingShipCount = 0);

        }

        #region Update

        internal void UpdateAdmiral(RawBasic rpAdmiral)
        {
            if (Admiral == null)
                Admiral = new Admiral(rpAdmiral);
            else
                Admiral.Update(rpAdmiral);
        }

        internal void UpdatePort(RawPort rpPort)
        {
            UpdateAdmiral(rpPort.Basic);
            Materials.Update(rpPort.Materials);

            UpdateShips(rpPort);
            UpdateRepairDocks(rpPort.RepairDocks);

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
        void UpdateRepairDocks(RawRepairDock[] rpDocks)
        {
            if (RepairDocks.UpdateRawData(rpDocks, r => new RepairDock(r), (rpData, rpRawData) => rpData.Update(rpRawData)))
                OnPropertyChanged(nameof(RepairDocks));
        }

        #endregion
    }
}

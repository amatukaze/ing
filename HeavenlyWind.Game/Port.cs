using Sakuno.KanColle.Amatsukaze.Game.Models;
using Sakuno.KanColle.Amatsukaze.Game.Models.Raw;
using Sakuno.KanColle.Amatsukaze.Game.Services;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;

namespace Sakuno.KanColle.Amatsukaze.Game
{
    public class Port : ModelBase
    {
        public Admiral Admiral { get; private set; }

        public Materials Materials { get; } = new Materials();

        public HashSet<int> ShipIDs { get; private set; }
        public IDTable<Ship> Ships { get; } = new IDTable<Ship>();

        int[] r_EvacuatedShipIDs;
        internal HashSet<int> EvacuatedShipIDs { get; } = new HashSet<int>();

        public FleetManager Fleets { get; } = new FleetManager();

        public IDTable<Equipment> Equipment { get; } = new IDTable<Equipment>();

        public IDTable<RepairDock> RepairDocks { get; } = new IDTable<RepairDock>();
        public IDTable<ConstructionDock> ConstructionDocks { get; } = new IDTable<ConstructionDock>();

        public QuestManager Quests { get; } = new QuestManager();

        internal Port()
        {
            SessionService.Instance.Subscribe("api_get_member/ship2", r =>
            {
                UpdateShips(r.Json["api_data"].ToObject<RawShip[]>());
                Fleets.Update(r.Json["api_data_deck"].ToObject<RawFleet[]>());
            });
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

            SessionService.Instance.Subscribe("api_req_kaisou/slot_exchange_index", r =>
            {
                Ship rShip;
                if (Ships.TryGetValue(int.Parse(r.Requests["api_id"]), out rShip))
                    rShip.UpdateEquipmentIDs(r.GetData<RawEquipmentIDs>().EquipmentIDs);
            });

            SessionService.Instance.Subscribe("api_req_kaisou/powerup", r =>
            {
                var rShipID = int.Parse(r.Requests["api_id"]);
                var rData = r.GetData<RawModernizationResult>();

                Ship rModernizedShip;
                if (Ships.TryGetValue(rShipID, out rModernizedShip))
                    rModernizedShip.Update(rData.Ship);

                var rConsumedShips = r.Requests["api_id_items"].Split(',').Select(rpID => Ships[int.Parse(rpID)]).ToArray();
                var rConsumedEquipment = rConsumedShips.SelectMany(rpShip => rpShip.EquipedEquipment).ToArray();

                foreach (var rEquipment in rConsumedEquipment)
                    Equipment.Remove(rEquipment);
                foreach (var rShip in rConsumedShips)
                    Ships.Remove(rShip);

                var rRecord = RecordService.Instance?.Fate;
                if (rRecord != null)
                {
                    rRecord.AddShipFate(rConsumedShips, Fate.ConsumedByModernization);
                    rRecord.AddEquipmentFate(rConsumedEquipment, Fate.ConsumedByModernization);
                }

                UpdateShipsCore();
                OnPropertyChanged(nameof(Equipment));
                Fleets.Update(rData.Fleets);
            });

            SessionService.Instance.Subscribe("api_req_kousyou/createship", r => ConstructionDocks[int.Parse(r.Requests["api_kdock_id"])].IsConstructionStarted = true);
            SessionService.Instance.Subscribe("api_req_kousyou/getship", r =>
            {
                var rData = r.GetData<RawConstructionResult>();

                UpdateConstructionDocks(rData.ConstructionDocks);
                AddEquipment(rData.Equipment);

                Ships.Add(new Ship(rData.Ship));
                UpdateShipsCore();
            });
            SessionService.Instance.Subscribe("api_req_kousyou/createship_speedchage", r =>
            {
                if (r.Requests["api_highspeed"] == "1")
                    ConstructionDocks[int.Parse(r.Requests["api_kdock_id"])].CompleteConstruction();
            });

            SessionService.Instance.Subscribe("api_req_kousyou/destroyship", r =>
            {
                Materials.Update(r.Json["api_data"]["api_material"].ToObject<int[]>());

                var rShip = Ships[int.Parse(r.Requests["api_ship_id"])];

                var rRecord = RecordService.Instance?.Fate;
                if (rRecord != null)
                {
                    rRecord.AddShipFate(rShip, Fate.Dismantled);
                    rRecord.AddEquipmentFate(rShip.EquipedEquipment, Fate.Dismantled);
                }

                foreach (var rEquipment in rShip.EquipedEquipment)
                    Equipment.Remove(rEquipment);
                OnPropertyChanged(nameof(Equipment));

                rShip.OwnerFleet?.Remove(rShip);
                Ships.Remove(rShip);
                UpdateShipsCore();
            });
            SessionService.Instance.Subscribe("api_req_kousyou/destroyitem2", r =>
            {
                var rEquipmentIDs = r.Requests["api_slotitem_ids"].Split(',').Select(int.Parse);

                RecordService.Instance?.Fate?.AddEquipmentFate(rEquipmentIDs.Select(rpID => Equipment[rpID]), Fate.Scrapped);

                foreach (var rEquipmentID in rEquipmentIDs)
                    Equipment.Remove(rEquipmentID);
                OnPropertyChanged(nameof(Equipment));

                var rMaterials = r.Json["api_data"]["api_get_material"].ToObject<int[]>();
                Materials.Fuel += rMaterials[0];
                Materials.Bullet += rMaterials[1];
                Materials.Steel += rMaterials[2];
                Materials.Bauxite += rMaterials[3];
            });

            SessionService.Instance.Subscribe("api_req_kousyou/remodel_slot", r =>
            {
                var rData = (RawImprovementResult)r.Data;

                Materials.Update(rData.Materials);

                Equipment rEquipment;
                if (rData.Success && Equipment.TryGetValue(rData.ImprovedEquipment.ID, out rEquipment))
                    rEquipment.Update(rData.ImprovedEquipment);

                if (rData.ConsumedEquipmentID != null)
                {
                    RecordService.Instance?.Fate?.AddEquipmentFate(rData.ConsumedEquipmentID.Select(rpID => Equipment[rpID]), Fate.ConsumedByImprovement);

                    foreach (var rEquipmentID in rData.ConsumedEquipmentID)
                        Equipment.Remove(rEquipmentID);
                    OnPropertyChanged(nameof(Equipment));
                }
            });

            SessionService.Instance.Subscribe("api_req_hokyu/charge", r =>
            {
                var rData = r.GetData<RawSupplyResult>();
                var rFleets = new HashSet<Fleet>();

                Materials.Update(rData.Materials);

                foreach (var rShipSupplyResult in rData.Ships)
                {
                    var rShip = Ships[rShipSupplyResult.ID];
                    rShip.Fuel = rShip.Fuel.Update(rShipSupplyResult.Fuel);
                    rShip.Bullet = rShip.Bullet.Update(rShipSupplyResult.Bullet);

                    if (rShip.OwnerFleet != null)
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

                if (rIsInstantRepair)
                    Materials.Bucket--;
            });
            SessionService.Instance.Subscribe("api_req_nyukyo/speedchange", r => RepairDocks[int.Parse(r.Requests["api_ndock_id"])].CompleteRepair());

            SessionService.Instance.Subscribe("api_req_combined_battle/battleresult", r =>
            {
                var rData = (RawBattleResult)r.Data;
                if (!rData.HasEvacuatedShip)
                    return;

                var rEvacuatedShipIndex = rData.EvacuatedShips.EvacuatedShipIndex[0] - 1;
                var rEvacuatedShipID = Fleets[rEvacuatedShipIndex < 6 ? 1 : 2].Ships[rEvacuatedShipIndex % 6].ID;

                var rEscortShipIndex = rData.EvacuatedShips.EscortShipIndex[0] - 1;
                var rEscortShipID =Fleets[rEscortShipIndex < 6 ? 1 : 2].Ships[rEscortShipIndex % 6].ID;

                r_EvacuatedShipIDs = new[] { rEvacuatedShipID, rEscortShipID };
            });
            SessionService.Instance.Subscribe("api_req_combined_battle/goback_port", delegate
            {
                if (KanColleGame.Current.Sortie == null || r_EvacuatedShipIDs == null || r_EvacuatedShipIDs.Length == 0)
                    return;

                EvacuatedShipIDs.Add(r_EvacuatedShipIDs[0]);
                EvacuatedShipIDs.Add(r_EvacuatedShipIDs[1]);
            });
            SessionService.Instance.Subscribe("api_get_member/ship_deck", _ => r_EvacuatedShipIDs = null);
            SessionService.Instance.Subscribe("api_port/port", _ => EvacuatedShipIDs.Clear());
        }

        #region Update

        internal void UpdateAdmiral(RawBasic rpAdmiral)
        {
            if (Admiral != null)
                Admiral.Update(rpAdmiral);
            else
            {
                Admiral = new Admiral(rpAdmiral);
                OnPropertyChanged(nameof(Admiral));
            }
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

        internal void UpdateEquipment(RawEquipment[] rpEquipment)
        {
            if (Equipment.UpdateRawData(rpEquipment, r => new Equipment(r), (rpData, rpRawData) => rpData.Update(rpRawData)))
                OnPropertyChanged(nameof(Equipment));
        }
        internal void AddEquipment(Equipment rpEquipment)
        {
            Equipment.Add(rpEquipment);
            OnPropertyChanged(nameof(Equipment));
        }
        internal void AddEquipment(RawEquipment[] rpRawData)
        {
            foreach (var rRawData in rpRawData)
                Equipment.Add(new Equipment(rRawData));
            OnPropertyChanged(nameof(Equipment));
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

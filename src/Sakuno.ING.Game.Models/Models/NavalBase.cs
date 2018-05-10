using System.Collections.Generic;
using System.Linq;

namespace Sakuno.ING.Game.Models
{
    public class NavalBase : BindableObject, ITableProvider
    {
        public NavalBase(GameListener listener)
        {
            MasterData = new MasterDataRoot(listener);
            Quests = new QuestManager(listener);
            _allEquipment = new IdTable<Equipment, IRawEquipment>(this);
            _buildingDocks = new IdTable<BuildingDock, IRawBuildingDock>(this);
            _repairingDocks = new IdTable<RepairingDock, IRawRepairingDock>(this);
            _useItems = new IdTable<UseItemCount, IRawUseItemCount>(this);
            _allShips = new IdTable<Ship, IRawShip>(this);
            _fleets = new IdTable<Fleet, IRawFleet>(this);
            _maps = new IdTable<Map, IRawMap>(this);
            _airForce = new IdTable<AirForceGroup, IRawAirForceGroup>(this);

            listener.AllEquipmentUpdated.Received += (_, msg) => _allEquipment.BatchUpdate(msg);
            listener.BuildingDockUpdated.Received += (_, msg) => _buildingDocks.BatchUpdate(msg);
            listener.UseItemUpdated.Received += (_, msg) => _useItems.BatchUpdate(msg);

            listener.AdmiralUpdated.Received += (_, msg) =>
            {
                if (Admiral == null)
                {
                    Admiral = new Admiral(msg.Id, this);
                    NotifyPropertyChanged(nameof(Admiral));
                }
                Admiral.Update(msg);
            };
            listener.MaterialsUpdated.Received += (_, msg) =>
            {
                var materials = Materials;
                msg.Apply(ref materials);
                Materials = materials;
            };
            listener.HomeportReturned.Received += (_, msg) => _allShips.BatchUpdate(msg.Ships);
            listener.CompositionChanged.Received += (_, msg) =>
            {
                var fleet = Fleets[msg.FleetId];
                if (fleet != null)
                {
                    if (msg.ShipId is int shipId)
                    {
                        var ship = AllShips.TryGetOrDummy(shipId);
                        fleet.ChangeComposition(msg.Index, ship, Fleets.FirstOrDefault(x => x.Ships.Contains(ship)));
                    }
                    else
                        fleet.ChangeComposition(msg.Index, null, null);
                }
            };
            listener.FleetsUpdated.Received += (_, msg) => _fleets.BatchUpdate(msg);
            listener.FleetPresetSelected.Received += (_, msg) => Fleets[msg.Id]?.Update(msg);
            listener.ShipEquipmentUdated.Received += (_, msg) => AllShips[msg.ShipId]?.UpdateEquipments(msg.EquipmentIds);
            listener.PartialFleetsUpdated.Received += (_, msg) => _fleets.BatchUpdate(msg, removal: false);
            listener.PartialShipsUpdated.Received += (_, msg) => _allShips.BatchUpdate(msg, removal: false);
            listener.RepairingDockUpdated.Received += (_, msg) => _repairingDocks.BatchUpdate(msg);
            listener.ShipSupplied.Received += (_, msg) =>
            {
                foreach (var raw in msg)
                    AllShips[raw.ShipId]?.Supply(raw);
            };

            listener.RepairStarted.Received += (_, msg) =>
            {
                if (msg.InstantRepair)
                    AllShips[msg.ShipId]?.SetRepaired();
            };
            listener.InstantRepaired.Received += (_, msg) =>
            {
                var dock = RepairingDocks[msg.DockId];
                dock.State = RepairingDockState.Empty;
                dock.RepairingShip = null;
            };
            listener.InstantBuilt.Received += (_, msg) => BuildingDocks[msg.DockId].State = BuildingDockState.BuildCompleted;
            listener.ShipBuildCompleted.Received += (_, msg) =>
            {
                _allEquipment.BatchUpdate(msg.Equipments, removal: false);
                _allShips.Add(msg.Ship);
            };
            listener.EquipmentCreated.Received += (_, msg) =>
            {
                if (msg.IsSuccess)
                    _allEquipment.Add(msg.Equipment);
            };
            listener.ShipDismantled.Received += (_, msg) => RemoveShip(msg.ShipIds, msg.DismantleEquipments);
            listener.EquipmentDismantled.Received += (_, msg) => RemoveEquipment(msg.EquipmentIds);
            listener.EquipmentImproved.Received += (_, msg) =>
            {
                if (msg.IsSuccess)
                    AllEquipment[msg.EquipmentId]?.Update(msg.UpdatedTo);
                RemoveEquipment(msg.ConsumedEquipmentId);
            };
            listener.ShipPoweruped.Received += (_, msg) =>
            {
                AllShips[msg.ShipId]?.Update(msg.ShipAfter);
                foreach (var id in msg.ConsumedShipIds)
                    _allShips.Remove(id);
            };

            listener.MapsUpdated.Received += (_, msg) => _maps.BatchUpdate(msg);
            listener.AirForceUpdated.Received += (_, msg) => _airForce.BatchUpdate(msg);
            listener.AirForcePlaneSet.Received += (_, msg) =>
            {
                var group = AirForce[(msg.MapAreaId << 16) + msg.AirForceId];
                group.Distance = msg.NewDistance;
                group.squadrons.BatchUpdate(msg.UpdatedSquadrons, removal: false);
            };
            listener.AirForceActionSet.Received += (_, msg) =>
            {
                foreach (var m in msg)
                    AirForce[(m.MapAreaId << 16) + m.AirForceId].Action = m.Action;
            };
            listener.AirForceSupplied.Received += (_, msg)
                => AirForce[(msg.MapAreaId << 16) + msg.AirForceId].squadrons.BatchUpdate(msg.UpdatedSquadrons, removal: false);
            listener.AirForceExpanded.Received += (_, msg) => _airForce.Add(msg);
        }

        private void RemoveShip(IEnumerable<int> shipIds, bool removeEquipment)
        {
            foreach (int id in shipIds)
            {
                var ship = AllShips[id];
                if (removeEquipment)
                    RemoveEquipment(ship.Slots.Where(x => !x.IsEmpty).Select(x => x.Equipment.Id));
                _allShips.Remove(ship);
            }
        }

        private void RemoveEquipment(IEnumerable<int> ids)
        {
            foreach (int id in ids)
                _allEquipment.Remove(id);
        }

        public MasterDataRoot MasterData { get; }
        public QuestManager Quests { get; }

        private readonly IdTable<Equipment, IRawEquipment> _allEquipment;
        public ITable<Equipment> AllEquipment => _allEquipment;

        private readonly IdTable<BuildingDock, IRawBuildingDock> _buildingDocks;
        public ITable<BuildingDock> BuildingDocks => _buildingDocks;

        private readonly IdTable<RepairingDock, IRawRepairingDock> _repairingDocks;
        public ITable<RepairingDock> RepairingDocks => _repairingDocks;

        private readonly IdTable<UseItemCount, IRawUseItemCount> _useItems;
        public ITable<UseItemCount> UseItems => _useItems;

        private readonly IdTable<Ship, IRawShip> _allShips;
        public ITable<Ship> AllShips => _allShips;

        private readonly IdTable<Fleet, IRawFleet> _fleets;
        public ITable<Fleet> Fleets => _fleets;

        private readonly IdTable<Map, IRawMap> _maps;
        public ITable<Map> Maps => _maps;

        private readonly IdTable<AirForceGroup, IRawAirForceGroup> _airForce;
        public ITable<AirForceGroup> AirForce => _airForce;

        public Admiral Admiral { get; private set; }

        private Materials _materials;
        public Materials Materials
        {
            get => _materials;
            set => Set(ref _materials, value);
        }

        public ITable<T> TryGetTable<T>()
        {
            var type = typeof(T);

            if (type == typeof(Equipment))
                return (ITable<T>)AllEquipment;

            if (type == typeof(BuildingDock))
                return (ITable<T>)BuildingDocks;

            if (type == typeof(RepairingDock))
                return (ITable<T>)RepairingDocks;

            if (type == typeof(UseItemCount))
                return (ITable<T>)UseItems;

            if (type == typeof(Ship))
                return (ITable<T>)AllShips;

            if (type == typeof(Fleet))
                return (ITable<T>)Fleets;

            if (type == typeof(Map))
                return (ITable<T>)Maps;

            if (type == typeof(AirForceGroup))
                return (ITable<T>)AirForce;

            return MasterData.TryGetTable<T>() ?? Quests.TryGetTable<T>();
        }
    }
}

using System.Collections.Generic;
using System.Linq;

namespace Sakuno.ING.Game.Models
{
    public class NavalBase : BindableObject
    {
        public NavalBase(IGameProvider listener)
        {
            MasterData = new MasterDataRoot(listener);
            Quests = new QuestManager(listener);
            _allEquipment = new IdTable<int, Equipment, IRawEquipment, NavalBase>(this);
            _buildingDocks = new IdTable<int, BuildingDock, IRawBuildingDock, NavalBase>(this);
            _repairingDocks = new IdTable<int, RepairingDock, IRawRepairingDock, NavalBase>(this);
            _useItems = new IdTable<int, UseItemCount, IRawUseItemCount, NavalBase>(this);
            _allShips = new IdTable<int, Ship, IRawShip, NavalBase>(this);
            _fleets = new IdTable<int, Fleet, IRawFleet, NavalBase>(this);
            _maps = new IdTable<int, Map, IRawMap, NavalBase>(this);
            _airForce = new IdTable<int, AirForceGroup, IRawAirForceGroup, NavalBase>(this);

            listener.AllEquipmentUpdated += (_, msg) => _allEquipment.BatchUpdate(msg);
            listener.BuildingDockUpdated += (_, msg) => _buildingDocks.BatchUpdate(msg);
            listener.UseItemUpdated += (_, msg) => _useItems.BatchUpdate(msg);

            listener.AdmiralUpdated += (_, msg) =>
            {
                if (Admiral == null)
                {
                    Admiral = new Admiral(msg.Id, this);
                    NotifyPropertyChanged(nameof(Admiral));
                }
                Admiral.Update(msg);
            };
            listener.MaterialsUpdated += (_, msg) =>
            {
                var materials = Materials;
                msg.Apply(ref materials);
                Materials = materials;
            };
            listener.HomeportReturned += (_, msg) => _allShips.BatchUpdate(msg.Ships);
            listener.CompositionChanged += (_, msg) =>
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
            listener.FleetsUpdated += (_, msg) => _fleets.BatchUpdate(msg);
            listener.FleetPresetSelected += (_, msg) => Fleets[msg.Id]?.Update(msg);
            listener.ShipEquipmentUdated += (_, msg) => AllShips[msg.ShipId]?.UpdateEquipments(msg.EquipmentIds);
            listener.PartialFleetsUpdated += (_, msg) => _fleets.BatchUpdate(msg, removal: false);
            listener.PartialShipsUpdated += (_, msg) => _allShips.BatchUpdate(msg, removal: false);
            listener.RepairingDockUpdated += (_, msg) => _repairingDocks.BatchUpdate(msg);
            listener.ShipSupplied += (_, msg) =>
            {
                foreach (var raw in msg)
                    AllShips[raw.ShipId]?.Supply(raw);
            };

            listener.RepairStarted += (_, msg) =>
            {
                if (msg.InstantRepair)
                    AllShips[msg.ShipId]?.SetRepaired();
            };
            listener.InstantRepaired += (_, msg) =>
            {
                var dock = RepairingDocks[msg];
                dock.State = RepairingDockState.Empty;
                dock.RepairingShip = null;
            };
            listener.InstantBuilt += (_, msg) => BuildingDocks[msg].State = BuildingDockState.BuildCompleted;
            listener.ShipBuildCompleted += (_, msg) =>
            {
                _allEquipment.BatchUpdate(msg.Equipments, removal: false);
                _allShips.Add(msg.Ship);
            };
            listener.EquipmentCreated += (_, msg) =>
            {
                if (msg.IsSuccess)
                    _allEquipment.Add(msg.Equipment);
            };
            listener.ShipDismantled += (_, msg) => RemoveShip(msg.ShipIds, msg.DismantleEquipments);
            listener.EquipmentDismantled += (_, msg) => RemoveEquipment(msg);
            listener.EquipmentImproved += (_, msg) =>
            {
                if (msg.IsSuccess)
                    AllEquipment[msg.EquipmentId]?.Update(msg.UpdatedTo);
                RemoveEquipment(msg.ConsumedEquipmentIds);
            };
            listener.ShipPoweruped += (_, msg) =>
            {
                AllShips[msg.ShipId]?.Update(msg.ShipAfter);
                foreach (var id in msg.ConsumedShipIds)
                    _allShips.Remove(id);
            };

            listener.MapsUpdated += (_, msg) => _maps.BatchUpdate(msg);
            listener.AirForceUpdated += (_, msg) => _airForce.BatchUpdate(msg);
            listener.AirForcePlaneSet += (_, msg) =>
            {
                var group = AirForce[(msg.MapAreaId << 16) + msg.AirForceId];
                group.Distance = msg.NewDistance;
                group.squadrons.BatchUpdate(msg.UpdatedSquadrons, removal: false);
            };
            listener.AirForceActionSet += (_, msg) =>
            {
                foreach (var m in msg)
                    AirForce[(m.MapAreaId << 16) + m.AirForceId].Action = m.Action;
            };
            listener.AirForceSupplied += (_, msg)
                => AirForce[(msg.MapAreaId << 16) + msg.AirForceId].squadrons.BatchUpdate(msg.UpdatedSquadrons, removal: false);
            listener.AirForceExpanded += (_, msg) => _airForce.Add(msg);
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

        private readonly IdTable<int, Equipment, IRawEquipment, NavalBase> _allEquipment;
        public ITable<int, Equipment> AllEquipment => _allEquipment;

        private readonly IdTable<int, BuildingDock, IRawBuildingDock, NavalBase> _buildingDocks;
        public ITable<int, BuildingDock> BuildingDocks => _buildingDocks;

        private readonly IdTable<int, RepairingDock, IRawRepairingDock, NavalBase> _repairingDocks;
        public ITable<int, RepairingDock> RepairingDocks => _repairingDocks;

        private readonly IdTable<int, UseItemCount, IRawUseItemCount, NavalBase> _useItems;
        public ITable<int, UseItemCount> UseItems => _useItems;

        private readonly IdTable<int, Ship, IRawShip, NavalBase> _allShips;
        public ITable<int, Ship> AllShips => _allShips;

        private readonly IdTable<int, Fleet, IRawFleet, NavalBase> _fleets;
        public ITable<int, Fleet> Fleets => _fleets;

        private readonly IdTable<int, Map, IRawMap, NavalBase> _maps;
        public ITable<int, Map> Maps => _maps;

        private readonly IdTable<int, AirForceGroup, IRawAirForceGroup, NavalBase> _airForce;
        public ITable<int, AirForceGroup> AirForce => _airForce;

        public Admiral Admiral { get; private set; }

        private Materials _materials;
        public Materials Materials
        {
            get => _materials;
            set => Set(ref _materials, value);
        }
    }
}

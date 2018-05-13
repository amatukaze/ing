using System;
using System.Collections.Generic;
using System.Linq;
using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Models
{
    public class NavalBase : BindableObject
    {
        public NavalBase(IGameProvider listener)
        {
            MasterData = new MasterDataRoot(listener);
            Quests = new QuestManager(listener);
            _allEquipment = new IdTable<EquipmentId, Equipment, IRawEquipment, NavalBase>(this);
            _buildingDocks = new IdTable<BuildingDockId, BuildingDock, IRawBuildingDock, NavalBase>(this);
            _repairingDocks = new IdTable<RepairingDockId, RepairingDock, IRawRepairingDock, NavalBase>(this);
            _useItems = new IdTable<UseItemId, UseItemCount, IRawUseItemCount, NavalBase>(this);
            _allShips = new IdTable<ShipId, Ship, IRawShip, NavalBase>(this);
            _fleets = new IdTable<FleetId, Fleet, IRawFleet, NavalBase>(this);
            _maps = new IdTable<MapId, Map, IRawMap, NavalBase>(this);
            _airForce = new IdTable<(MapAreaId MapArea, AirForceGroupId GroupId), AirForceGroup, IRawAirForceGroup, NavalBase>(this);

            listener.AllEquipmentUpdated += (t, msg) => _allEquipment.BatchUpdate(msg, t);
            listener.BuildingDockUpdated += (t, msg) => _buildingDocks.BatchUpdate(msg, t);
            listener.UseItemUpdated += (t, msg) => _useItems.BatchUpdate(msg, t);

            listener.AdmiralUpdated += (t, msg) =>
            {
                if (Admiral == null)
                {
                    Admiral = new Admiral(msg, this, t);
                    NotifyPropertyChanged(nameof(Admiral));
                }
                else
                    Admiral.Update(msg, t);
            };
            listener.MaterialsUpdated += (t, msg) =>
            {
                var materials = Materials;
                msg.Apply(ref materials);
                Materials = materials;
            };
            listener.HomeportReturned += (t, msg) => _allShips.BatchUpdate(msg.Ships, t);
            listener.CompositionChanged += (t, msg) =>
            {
                var fleet = Fleets[msg.FleetId];
                if (msg.ShipId > 0)
                {
                    var ship = AllShips.TryGetOrDummy(msg.ShipId);
                    fleet.ChangeComposition(msg.Index, ship, Fleets.FirstOrDefault(x => x.Ships.Contains(ship)));
                }
                else
                    fleet.ChangeComposition(msg.Index, null, null);
            };
            listener.FleetsUpdated += (t, msg) => _fleets.BatchUpdate(msg, t);
            listener.FleetPresetSelected += (t, msg) => Fleets[msg.Id].Update(msg, t);
            listener.ShipEquipmentUdated += (t, msg) => AllShips[msg.ShipId].UpdateEquipments(msg.EquipmentIds);
            listener.ShipExtraSlotOpened += (t, msg) => AllShips[msg].ExtraSlot = new Slot();
            listener.PartialFleetsUpdated += (t, msg) => _fleets.BatchUpdate(msg, t, removal: false);
            listener.PartialShipsUpdated += (t, msg) => _allShips.BatchUpdate(msg, t, removal: false);
            listener.RepairingDockUpdated += (t, msg) => _repairingDocks.BatchUpdate(msg, t);
            listener.ShipSupplied += (t, msg) =>
            {
                foreach (var raw in msg)
                    AllShips[raw.ShipId]?.Supply(raw);
            };

            listener.RepairStarted += (t, msg) =>
            {
                if (msg.InstantRepair)
                    AllShips[msg.ShipId]?.SetRepaired();
            };
            listener.InstantRepaired += (t, msg) =>
            {
                var dock = RepairingDocks[msg];
                dock.State = RepairingDockState.Empty;
                dock.RepairingShip = null;
            };
            listener.InstantBuilt += (t, msg) => BuildingDocks[msg].State = BuildingDockState.BuildCompleted;
            listener.ShipBuildCompleted += (t, msg) =>
            {
                _allEquipment.BatchUpdate(msg.Equipments, t, removal: false);
                _allShips.Add(msg.Ship, t);
            };
            listener.EquipmentCreated += (t, msg) =>
            {
                if (msg.IsSuccess)
                    _allEquipment.Add(msg.Equipment, t);
            };
            listener.ShipDismantled += (t, msg) => RemoveShip(msg.ShipIds, msg.DismantleEquipments, t);
            listener.EquipmentDismantled += (t, msg) => RemoveEquipment(msg, t);
            listener.EquipmentImproved += (t, msg) =>
            {
                if (msg.IsSuccess)
                    AllEquipment[msg.EquipmentId].Update(msg.UpdatedTo, t);
                RemoveEquipment(msg.ConsumedEquipmentIds, t);
            };
            listener.ShipPoweruped += (t, msg) =>
            {
                AllShips[msg.ShipId].Update(msg.ShipAfter, t);
                foreach (var id in msg.ConsumedShipIds)
                    _allShips.Remove(id);
            };

            listener.MapsUpdated += (t, msg) => _maps.BatchUpdate(msg, t);
            listener.AirForceUpdated += (t, msg) => _airForce.BatchUpdate(msg, t);
            listener.AirForcePlaneSet += (t, msg) =>
            {
                var group = AirForce[(msg.MapAreaId, msg.GroupId)];
                group.Distance = msg.NewDistance;
                group.squadrons.BatchUpdate(msg.UpdatedSquadrons, t, removal: false);
            };
            listener.AirForceActionSet += (t, msg) =>
            {
                foreach (var m in msg)
                    AirForce[(m.MapAreaId, m.GroupId)].Action = m.Action;
            };
            listener.AirForceSupplied += (t, msg)
                => AirForce[(msg.MapAreaId, msg.GroupId)].squadrons.BatchUpdate(msg.UpdatedSquadrons, t, removal: false);
            listener.AirForceExpanded += (t, msg) => _airForce.Add(msg, t);
        }

        private void RemoveShip(IEnumerable<ShipId> shipIds, bool removeEquipment, DateTimeOffset timeStamp)
        {
            foreach (var id in shipIds)
            {
                var ship = AllShips[id];
                if (removeEquipment)
                    RemoveEquipment(ship.Slots.Where(x => !x.IsEmpty).Select(x => x.Equipment.Id), timeStamp);
                _allShips.Remove(ship);
            }
        }

        private void RemoveEquipment(IEnumerable<EquipmentId> ids, DateTimeOffset timeStamp)
        {
            foreach (var id in ids)
                _allEquipment.Remove(id);
        }

        public MasterDataRoot MasterData { get; }
        public QuestManager Quests { get; }

        private readonly IdTable<EquipmentId, Equipment, IRawEquipment, NavalBase> _allEquipment;
        public ITable<EquipmentId, Equipment> AllEquipment => _allEquipment;

        private readonly IdTable<BuildingDockId, BuildingDock, IRawBuildingDock, NavalBase> _buildingDocks;
        public ITable<BuildingDockId, BuildingDock> BuildingDocks => _buildingDocks;

        private readonly IdTable<RepairingDockId, RepairingDock, IRawRepairingDock, NavalBase> _repairingDocks;
        public ITable<RepairingDockId, RepairingDock> RepairingDocks => _repairingDocks;

        private readonly IdTable<UseItemId, UseItemCount, IRawUseItemCount, NavalBase> _useItems;
        public ITable<UseItemId, UseItemCount> UseItems => _useItems;

        private readonly IdTable<ShipId, Ship, IRawShip, NavalBase> _allShips;
        public ITable<ShipId, Ship> AllShips => _allShips;

        private readonly IdTable<FleetId, Fleet, IRawFleet, NavalBase> _fleets;
        public ITable<FleetId, Fleet> Fleets => _fleets;

        private readonly IdTable<MapId, Map, IRawMap, NavalBase> _maps;
        public ITable<MapId, Map> Maps => _maps;

        private readonly IdTable<(MapAreaId MapArea, AirForceGroupId GroupId), AirForceGroup, IRawAirForceGroup, NavalBase> _airForce;
        public ITable<(MapAreaId MapArea, AirForceGroupId GroupId), AirForceGroup> AirForce => _airForce;

        public Admiral Admiral { get; private set; }

        private Materials _materials;
        public Materials Materials
        {
            get => _materials;
            set => Set(ref _materials, value);
        }
    }
}

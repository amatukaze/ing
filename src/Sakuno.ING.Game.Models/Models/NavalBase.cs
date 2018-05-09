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

            listener.AllEquipmentUpdated.Received += msg => _allEquipment.BatchUpdate(msg.Message);
            listener.BuildingDockUpdated.Received += msg => _buildingDocks.BatchUpdate(msg.Message);
            listener.UseItemUpdated.Received += msg => _useItems.BatchUpdate(msg.Message);

            listener.AdmiralUpdated.Received += msg =>
            {
                if (Admiral == null)
                {
                    Admiral = new Admiral(msg.Message.Id, this);
                    NotifyPropertyChanged(nameof(Admiral));
                }
                Admiral.Update(msg.Message);
            };
            listener.MaterialsUpdated.Received += msg =>
            {
                var materials = Materials;
                msg.Message.Apply(ref materials);
                Materials = materials;
            };
            listener.HomeportReturned.Received += msg => _allShips.BatchUpdate(msg.Message.Ships);
            listener.CompositionChanged.Received += msg =>
            {
                var fleet = Fleets[msg.Message.FleetId];
                if (fleet != null)
                {
                    if (msg.Message.ShipId is int shipId)
                    {
                        var ship = AllShips.TryGetOrDummy(shipId);
                        fleet.ChangeComposition(msg.Message.Index, ship, Fleets.FirstOrDefault(x => x.Ships.Contains(ship)));
                    }
                    else
                        fleet.ChangeComposition(msg.Message.Index, null, null);
                }
            };
            listener.FleetsUpdated.Received += msg => _fleets.BatchUpdate(msg.Message);
            listener.FleetPresetSelected.Received += msg => Fleets[msg.Message.Id]?.Update(msg.Message);
            listener.ShipEquipmentUdated.Received += msg => AllShips[msg.Message.ShipId]?.UpdateEquipments(msg.Message.EquipmentIds);
            listener.PartialFleetsUpdated.Received += msg => _fleets.BatchUpdate(msg.Message, removal: false);
            listener.PartialShipsUpdated.Received += msg => _allShips.BatchUpdate(msg.Message, removal: false);
            listener.RepairingDockUpdated.Received += msg => _repairingDocks.BatchUpdate(msg.Message);
            listener.ShipSupplied.Received += msg =>
            {
                foreach (var raw in msg.Message)
                    AllShips[raw.ShipId]?.Supply(raw);
            };

            listener.RepairStarted.Received += msg =>
            {
                if (msg.Message.InstantRepair)
                    AllShips[msg.Message.ShipId]?.SetRepaired();
            };
            listener.InstantRepaired.Received += msg =>
            {
                var dock = RepairingDocks[msg.Message.DockId];
                dock.State = RepairingDockState.Empty;
                dock.RepairingShip = null;
            };
            listener.InstantBuilt.Received += msg => BuildingDocks[msg.Message.DockId].State = BuildingDockState.BuildCompleted;
            listener.ShipBuildCompleted.Received += msg =>
            {
                _allEquipment.BatchUpdate(msg.Message.Equipments, removal: false);
                _allShips.Add(msg.Message.Ship);
            };
            listener.EquipmentCreated.Received += msg =>
            {
                if (msg.Message.IsSuccess)
                    _allEquipment.Add(msg.Message.Equipment);
            };
            listener.ShipDismantled.Received += msg => RemoveShip(msg.Message.ShipIds, msg.Message.DismantleEquipments);
            listener.EquipmentDismantled.Received += msg => RemoveEquipment(msg.Message.EquipmentIds);
            listener.EquipmentImproved.Received += msg =>
            {
                if (msg.Message.IsSuccess)
                    AllEquipment[msg.Message.EquipmentId]?.Update(msg.Message.UpdatedTo);
                RemoveEquipment(msg.Message.ConsumedEquipmentId);
            };
            listener.ShipPoweruped.Received += msg =>
            {
                AllShips[msg.Message.ShipId]?.Update(msg.Message.ShipAfter);
                foreach (var id in msg.Message.ConsumedShipIds)
                    _allShips.Remove(id);
            };

            listener.MapsUpdated.Received += msg => _maps.BatchUpdate(msg.Message);
            listener.AirForceUpdated.Received += msg => _airForce.BatchUpdate(msg.Message);
            listener.AirForcePlaneSet.Received += msg =>
            {
                var m = msg.Message;
                var group = AirForce[(m.MapAreaId << 16) + m.AirForceId];
                group.Distance = m.NewDistance;
                group.squadrons.BatchUpdate(m.UpdatedSquadrons, removal: false);
            };
            listener.AirForceActionSet.Received += msg =>
            {
                foreach (var m in msg.Message)
                    AirForce[(m.MapAreaId << 16) + m.AirForceId].Action = m.Action;
            };
            listener.AirForceSupplied.Received += msg
                => AirForce[(msg.Message.MapAreaId << 16) + msg.Message.AirForceId].squadrons.BatchUpdate(msg.Message.UpdatedSquadrons, removal: false);
            listener.AirForceExpanded.Received += msg => _airForce.Add(msg.Message);
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

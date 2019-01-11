using System;
using System.Collections.Generic;
using System.Linq;
using Sakuno.ING.Composition;
using Sakuno.ING.Game.Events;
using Sakuno.ING.Game.Models.MasterData;
using Sakuno.ING.Localization;
using Sakuno.ING.Messaging;
using Sakuno.ING.Timing;

namespace Sakuno.ING.Game.Models
{
    [Export(typeof(NavalBase))]
    public class NavalBase : BindableObject
    {
        public ILocalizationService Localization { get; }

        public NavalBase(GameProvider listener, ILocalizationService localization, ITimingService timingService)
        {
            Localization = localization;

            MasterData = new MasterDataRoot(listener, localization);
            Quests = new QuestManager(listener, localization);
            _allEquipment = new IdTable<EquipmentId, Equipment, RawEquipment, NavalBase>(this);
            _buildingDocks = new IdTable<BuildingDockId, BuildingDock, RawBuildingDock, NavalBase>(this);
            _repairingDocks = new IdTable<RepairingDockId, RepairingDock, RawRepairingDock, NavalBase>(this);
            _useItems = new IdTable<UseItemId, UseItemCount, RawUseItemCount, NavalBase>(this);
            _allShips = new IdTable<ShipId, Ship, RawShip, NavalBase>(this);
            _fleets = new IdTable<FleetId, Fleet, RawFleet, NavalBase>(this);
            _maps = new IdTable<MapId, Map, RawMap, NavalBase>(this);
            _airForce = new IdTable<(MapAreaId MapArea, AirForceGroupId GroupId), AirForceGroup, RawAirForceGroup, NavalBase>(this);

            listener.AllEquipmentUpdated += (t, msg) => _allEquipment.BatchUpdate(msg, t);
            listener.BuildingDockUpdated += (t, msg) => _buildingDocks.BatchUpdate(msg, t);
            listener.UseItemUpdated += (t, msg) => _useItems.BatchUpdate(msg, t);

            listener.AdmiralUpdated += (t, msg) =>
            {
                if (Admiral?.Id != msg.Id)
                {
                    var @new = new Admiral(msg, this, t);
                    AdmiralChanging?.Invoke(t, Admiral, @new);
                    Admiral = @new;
                    NotifyPropertyChanged(nameof(Admiral));
                }
                else
                    Admiral.Update(msg, t);
            };
            listener.MaterialsUpdated += (t, msg) =>
            {
                var oldMaterials = Materials;
                var materials = oldMaterials;
                msg.Apply(ref materials);
                if (Materials != materials)
                {
                    Materials = materials;
                    MaterialsUpdating?.Invoke(t, oldMaterials, materials, msg.Reason);
                }
            };
            listener.HomeportReturned += (t, msg) =>
            {
                _allShips.BatchUpdate(msg.Ships, t);
                CombinedFleet = msg.CombinedFleetType;
            };
            listener.CompositionChanged += (t, msg) =>
            {
                var fleet = Fleets[msg.FleetId];
                if (msg.ShipId is ShipId shipId)
                {
                    var ship = AllShips[shipId];
                    fleet.ChangeComposition(msg.Index, ship);
                }
                else
                    fleet.ChangeComposition(msg.Index, null);
            };
            listener.FleetsUpdated += (t, msg) => _fleets.BatchUpdate(msg, t);
            listener.FleetPresetSelected += (t, msg) => Fleets[msg.Id].Update(msg, t);
            listener.ShipEquipmentUpdated += (t, msg) => AllShips[msg.ShipId].UpdateEquipments(msg.EquipmentIds);
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
                var ship = AllShips[msg.ShipId];
                if (ship == null)
                    return;

                if (msg.InstantRepair)
                    ship.SetRepaired();

                var oldMaterials = Materials;
                var materials = oldMaterials;
                materials.Fuel -= ship.RepairingCost.Fuel;
                materials.Steel -= ship.RepairingCost.Steel;
                Materials = materials;
                MaterialsUpdating?.Invoke(t, oldMaterials, materials, MaterialsChangeReason.ShipRepair);
            };
            listener.InstantRepaired += (t, msg) => RepairingDocks[msg].Instant();
            listener.InstantBuilt += (t, msg) =>
            {
                var dock = BuildingDocks[msg];
                dock.Instant();

                var oldMaterials = Materials;
                var materials = oldMaterials;
                materials.InstantBuild -= dock.IsLSC ? 10 : 1;
                Materials = materials;
                MaterialsUpdating?.Invoke(t, oldMaterials, materials, MaterialsChangeReason.InstantBuilt);
            };
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
            listener.ShipDismantled += (t, msg) =>
            {
                var removed = RemoveShips(msg.ShipIds, msg.DismantleEquipments, t);
                ShipDismantling?.Invoke(t, removed, msg.DismantleEquipments);
            };
            listener.EquipmentDismantled += (t, msg) =>
            {
                var removed = RemoveEquipment(msg, t);
                EquipmentDismantling?.Invoke(t, removed);
            };
            listener.EquipmentImproved += (t, msg) =>
            {
                var consumed = msg.ConsumedEquipmentIds != null ? RemoveEquipment(msg.ConsumedEquipmentIds, t) : null;
                var original = AllEquipment[msg.EquipmentId];
                EquipmentImproving?.Invoke(t, original, msg.UpdatedTo, consumed, msg.IsSuccess);
                if (msg.IsSuccess)
                    original.Update(msg.UpdatedTo, t);
            };
            listener.ShipPoweruped += (t, msg) =>
            {
                var consumed = RemoveShips(msg.ConsumedShipIds, true, t);
                var original = AllShips[msg.ShipId];
                ShipPoweruping?.Invoke(t, original, msg.UpdatedTo, consumed);
                original.Update(msg.UpdatedTo, t);
            };

            listener.MapsUpdated += (t, msg) => _maps.BatchUpdate(msg, t);
            listener.AirForceUpdated += (t, msg) => _airForce.BatchUpdate(msg, t);
            listener.AirForcePlaneSet += (t, msg) =>
            {
                var group = AirForce[(msg.MapAreaId, msg.GroupId)];
                group.DistanceBase = msg.NewDistanceBase;
                group.DistanceBonus = msg.NewDistanceBonus;
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

            if (timingService != null)
                timingService.Elapsed += t =>
                {
                    foreach (var f in Fleets) f.UpdateTimer(t);
                    foreach (var b in BuildingDocks) b.UpdateTimer(t);
                    foreach (var r in RepairingDocks) r.UpdateTimer(t);
                };
        }

        private IReadOnlyCollection<Ship> RemoveShips(IEnumerable<ShipId> shipIds, bool removeEquipment, DateTimeOffset timeStamp)
            => shipIds.Select(id =>
                {
                    var ship = AllShips[id];
                    if (removeEquipment)
                        RemoveEquipment(ship.Slots.Where(x => !x.IsEmpty).Select(x => x.Equipment.Id), timeStamp);
                    _allShips.Remove(ship);
                    ship.Fleet?.Remove(ship);
                    return ship;
                }).ToArray();

        private IReadOnlyCollection<Equipment> RemoveEquipment(IEnumerable<EquipmentId> ids, DateTimeOffset timeStamp)
            => ids.Select(_allEquipment.Remove).ToArray();

        public MasterDataRoot MasterData { get; }
        public QuestManager Quests { get; }

        private readonly IdTable<EquipmentId, Equipment, RawEquipment, NavalBase> _allEquipment;
        public ITable<EquipmentId, Equipment> AllEquipment => _allEquipment;

        private readonly IdTable<BuildingDockId, BuildingDock, RawBuildingDock, NavalBase> _buildingDocks;
        public ITable<BuildingDockId, BuildingDock> BuildingDocks => _buildingDocks;

        private readonly IdTable<RepairingDockId, RepairingDock, RawRepairingDock, NavalBase> _repairingDocks;
        public ITable<RepairingDockId, RepairingDock> RepairingDocks => _repairingDocks;

        private readonly IdTable<UseItemId, UseItemCount, RawUseItemCount, NavalBase> _useItems;
        public ITable<UseItemId, UseItemCount> UseItems => _useItems;

        private readonly IdTable<ShipId, Ship, RawShip, NavalBase> _allShips;
        public ITable<ShipId, Ship> AllShips => _allShips;

        private readonly IdTable<FleetId, Fleet, RawFleet, NavalBase> _fleets;
        public ITable<FleetId, Fleet> Fleets => _fleets;

        private readonly IdTable<MapId, Map, RawMap, NavalBase> _maps;
        public ITable<MapId, Map> Maps => _maps;

        private readonly IdTable<(MapAreaId MapArea, AirForceGroupId GroupId), AirForceGroup, RawAirForceGroup, NavalBase> _airForce;
        public ITable<(MapAreaId MapArea, AirForceGroupId GroupId), AirForceGroup> AirForce => _airForce;

        public Admiral Admiral { get; private set; }

        public Ship Secretary => Fleets.First().Ships[0];

        private Materials _materials;
        public Materials Materials
        {
            get => _materials;
            set => Set(ref _materials, value);
        }

        private CombinedFleetType _combinedFleet;
        public CombinedFleetType CombinedFleet
        {
            get => _combinedFleet;
            set => Set(ref _combinedFleet, value);
        }

        public event AdmiralChanging AdmiralChanging;
        public event MaterialsUpdatingHandler MaterialsUpdating;
        public event ShipDismantlingHandler ShipDismantling;
        public event ShipPowerupingHandler ShipPoweruping;
        public event EquipmentDismantlingHandler EquipmentDismantling;
        public event EquipmentImprovingHandler EquipmentImproving;
    }

    public delegate void AdmiralChanging(DateTimeOffset timeStamp, Admiral oldAdmiral, Admiral newAdmiral);
    public delegate void MaterialsUpdatingHandler(DateTimeOffset timeStamp, Materials oldMaterials, Materials newMaterials, MaterialsChangeReason reason);
    public delegate void ShipDismantlingHandler(DateTimeOffset timeStamp, IReadOnlyCollection<Ship> ships, bool dismantleEquipments);
    public delegate void ShipPowerupingHandler(DateTimeOffset timeStamp, Ship original, RawShip updatedTo, IReadOnlyCollection<Ship> consumed);
    public delegate void EquipmentDismantlingHandler(DateTimeOffset timeStamp, IReadOnlyCollection<Equipment> equipments);
    public delegate void EquipmentImprovingHandler(DateTimeOffset timeStamp, Equipment original, RawEquipment updatedTo, IReadOnlyCollection<Equipment> consumed, bool isSuccess);
}

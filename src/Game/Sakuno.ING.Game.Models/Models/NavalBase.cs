using Sakuno.ING.Composition;
using Sakuno.ING.Game.Models.Events;
using Sakuno.ING.Game.Models.MasterData;
using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Sakuno.ING.Game.Models
{
    [Export]
    public class NavalBase : BindableObject
    {
        public MasterDataRoot MasterData { get; }

        private readonly IdTable<ConstructionDockId, ConstructionDock, RawConstructionDock, NavalBase> _constructionDocks;
        public ITable<ConstructionDockId, ConstructionDock> ConstructionDocks => _constructionDocks;

        private readonly IdTable<RepairDockId, RepairDock, RawRepairDock, NavalBase> _repairDocks;
        public ITable<RepairDockId, RepairDock> RepairDocks => _repairDocks;

        private readonly IdTable<UseItemId, UseItemCount, RawUseItemCount, NavalBase> _useItems;
        public ITable<UseItemId, UseItemCount> UseItems => _useItems;

        private readonly IdTable<SlotItemId, PlayerSlotItem, RawSlotItem, NavalBase> _slotItems;
        public ITable<SlotItemId, PlayerSlotItem> SlotItems => _slotItems;

        private readonly IdTable<ShipId, PlayerShip, RawShip, NavalBase> _ships;
        public ITable<ShipId, PlayerShip> Ships => _ships;

        private readonly IdTable<FleetId, PlayerFleet, RawFleet, NavalBase> _fleets;
        public ITable<FleetId, PlayerFleet> Fleets => _fleets;

        private readonly IdTable<MapId, Map, RawMap, NavalBase> _maps;
        public ITable<MapId, Map> Maps => _maps;

        private readonly IdTable<(MapAreaId MapArea, AirForceGroupId Group), AirForceGroup, RawAirForceGroup, NavalBase> _airForceGroups;
        public ITable<(MapAreaId MapArea, AirForceGroupId Group), AirForceGroup> AirForceGroups => _airForceGroups;

        public IObservable<Admiral> Admiral { get; }
        public IObservable<Materials> Materials { get; }

        public NavalBase(GameProvider provider)
        {
            MasterData = new MasterDataRoot(provider);

            _constructionDocks = new IdTable<ConstructionDockId, ConstructionDock, RawConstructionDock, NavalBase>(this);
            _repairDocks = new IdTable<RepairDockId, RepairDock, RawRepairDock, NavalBase>(this);
            _useItems = new IdTable<UseItemId, UseItemCount, RawUseItemCount, NavalBase>(this);
            _slotItems = new IdTable<SlotItemId, PlayerSlotItem, RawSlotItem, NavalBase>(this);
            _ships = new IdTable<ShipId, PlayerShip, RawShip, NavalBase>(this);
            _fleets = new IdTable<FleetId, PlayerFleet, RawFleet, NavalBase>(this);
            _maps = new IdTable<MapId, Map, RawMap, NavalBase>(this);
            _airForceGroups = new IdTable<(MapAreaId MapArea, AirForceGroupId Group), AirForceGroup, RawAirForceGroup, NavalBase>(this);

            Admiral = provider.AdmiralUpdated.Scan((Admiral)null!, (admiral, message) =>
            {
                if (admiral?.Id != message.Id)
                    return new Admiral(message, this);

                admiral.Update(message);
                return admiral;
            });

            provider.ConstructionDocksUpdated.Subscribe(message => _constructionDocks.BatchUpdate(message));
            provider.RepairDocksUpdate.Subscribe(message => _repairDocks.BatchUpdate(message));
            provider.UseItemsUpdated.Subscribe(message => _useItems.BatchUpdate(message));
            provider.SlotItemsUpdated.Subscribe(message => _slotItems.BatchUpdate(message));
            provider.ShipsUpdate.Subscribe(message => _ships.BatchUpdate(message));
            provider.FleetsUpdate.Subscribe(message => _fleets.BatchUpdate(message));
            provider.MapsUpdated.Subscribe(message => _maps.BatchUpdate(message));
            provider.AirForceGroupsUpdated.Subscribe(message => _airForceGroups.BatchUpdate(message));

            provider.ShipUpdate.Subscribe(message => _ships[message.Id].Update(message));
            provider.FleetUpdate.Subscribe(message => _fleets[message.Id].Update(message));

            provider.FleetCompositionChanged.Subscribe(message => _fleets[message.FleetId].ChangeComposition(message.Index, _ships[message.ShipId]));

            provider.ShipSupplied.Subscribe(message => _ships[message.Id].Supply(message));

            provider.ShipModernization.Subscribe(message =>
            {
                foreach (var shipId in message.ConsumedShipIds)
                    RemoveShip(shipId, message.RemoveSlotItems);

                _ships[message.ShipId].Update(message.NewRawData);
            });

            provider.InstantRepairUsed.Subscribe(message => _repairDocks[message].InstantRepair());

            provider.ShipConstructed.Subscribe(message =>
            {
                _slotItems.BatchUpdate(message.SlotItems, false);
                _ships.Add(message.Ship);
            });
            provider.SlotItemsDeveloped.Where(message => message.IsSuccessful).Subscribe(message =>
            {
                foreach (var slotItem in message.SlotItems)
                    if (slotItem is not null)
                        _slotItems.Add(slotItem);
            });

            var instantConstructionUsed = provider.InstantConstructionUsed.Select(message => _constructionDocks[message]);
            instantConstructionUsed.Subscribe(message => message.InstantBuild());

            provider.ShipsDismantled.Subscribe(message =>
            {
                foreach (var shipId in message.ShipIds)
                    RemoveShip(shipId, message.RemoveSlotItems);
            });
            provider.SlotItemsScrapped.Subscribe(message =>
            {
                foreach (var slotItemId in message)
                    _slotItems.Remove(slotItemId);
            });

            provider.SlotItemImproved.Subscribe(message =>
            {
                if (message.IsSuccessful)
                    _slotItems[message.SlotItemId].Update(message.NewRawData);

                foreach (var slotItemId in message.ConsumedSlotItemIds)
                    _slotItems.Remove(slotItemId);
            });

            provider.AirForceActionUpdated.Subscribe(message => AirForceGroups[(message.MapAreaId, message.GroupId)].Action = message.Action);

            var materials = new Subject<Materials>();
            var materialUpdate = Observable.Merge(new[]
            {
                provider.MaterialUpdate,
                instantConstructionUsed.Select(message => new InstantConstructionMaterialUpdate(message.IsLSC)),
            });
            materialUpdate.Scan(new Materials(), (materials, message) =>
            {
                message.Apply(materials);
                return materials;
            }).Subscribe(materials);

            Materials = materials.AsObservable();
        }

        private void RemoveShip(ShipId shipId, bool removeSlotItems)
        {
            var ship = _ships.Remove(shipId) ?? throw new InvalidOperationException();

            foreach (var fleet in _fleets)
                if (fleet.Remove(ship))
                    break;

            if (!removeSlotItems)
                return;

            foreach (var slot in ship.Slots)
                if (slot.PlayerSlotItem is not null)
                    _slotItems.Remove(slot.PlayerSlotItem);
        }
    }
}

using Sakuno.ING.Composition;
using Sakuno.ING.Game.Models.Events;
using Sakuno.ING.Game.Models.MasterData;
using System;
using System.Collections.Generic;
using System.Linq;
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

        private readonly BehaviorSubject<Admiral> _admiralUpdated = new(null!);
        private IObservable<Admiral>? _admiralUpdatedObservable;
        public IObservable<Admiral> AdmiralUpdated => _admiralUpdatedObservable ??= _admiralUpdated.AsObservable().Where(r => r is not null);
        public Admiral Admiral => _admiralUpdated.Value;

        private readonly BehaviorSubject<Materials> _materialsUpdated = new(new());
        private IObservable<Materials>? _materialsUpdatedObservable;
        public IObservable<Materials> MaterialsUpdated => _materialsUpdatedObservable ??= _materialsUpdated.AsObservable();
        public Materials Materials => _materialsUpdated.Value;

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

            provider.ConstructionDocksUpdated.Subscribe(message => _constructionDocks.BatchUpdate(message));
            provider.RepairDocksUpdated.Subscribe(message => _repairDocks.BatchUpdate(message));
            provider.UseItemsUpdated.Subscribe(message => _useItems.BatchUpdate(message));
            provider.SlotItemsUpdated.Subscribe(message => _slotItems.BatchUpdate(message));
            provider.ShipsUpdated.Subscribe(message => _ships.BatchUpdate(message));
            provider.FleetsUpdated.Subscribe(message => _fleets.BatchUpdate(message));
            provider.MapsUpdated.Subscribe(message => _maps.BatchUpdate(message));
            provider.AirForceGroupsUpdated.Subscribe(message => _airForceGroups.BatchUpdate(message));

            provider.PartialShipsUpdated.Subscribe(messages =>
            {
                foreach (var message in messages)
                    _ships[message.Id].Update(message);
            });
            provider.PartialFleetsUpdated.Subscribe(messages =>
            {
                foreach (var message in messages)
                    _fleets[message.Id].Update(message);
            });

            var shipsRemoved = new Subject<IEnumerable<ShipId>>();
            var slotItemsRemoved = new Subject<IEnumerable<SlotItemId>>();

            shipsRemoved.Subscribe(_ships.RemoveIds);
            slotItemsRemoved.Subscribe(_slotItems.RemoveIds);

            provider.ShipsRemoved.Subscribe(shipsRemoved);
            provider.SlotItemsRemoved.Subscribe(slotItemsRemoved);

            provider.ShipsAndSlotItemsRemoved.Subscribe(shipIds =>
            {
                var slotItems = new List<SlotItemId>(shipIds.Length * 4 + 2);

                foreach (var shipId in shipIds)
                {
                    if (_ships[shipId] is not { } ship)
                        continue;

                    foreach (var slot in ship.Slots)
                        if (slot.PlayerSlotItem is { } slotItem)
                            slotItems.Add(slotItem.Id);

                    if (ship.ExtraSlot is { PlayerSlotItem: { } extraSlotItem })
                        slotItems.Add(extraSlotItem.Id);
                }

                shipsRemoved.OnNext(shipIds);
                slotItemsRemoved.OnNext(slotItems);
            });

            provider.FleetCompositionChanged.Subscribe(message => _fleets[message.FleetId].ChangeComposition(message.Index, _ships[message.ShipId]));

            provider.ShipsSupplied.Subscribe(messages =>
            {
                foreach (var message in messages)
                    _ships[message.Id].Supply(message);
            });

            provider.InstantRepairUsed.Subscribe(message => _repairDocks[message].InstantRepair());

            var instantConstructionUsed = provider.InstantConstructionUsed.Select(message => _constructionDocks[message]);
            instantConstructionUsed.Subscribe(message => message.InstantBuild());

            provider.AirForceGroupActionUpdated.Subscribe(messages =>
            {
                foreach (var message in messages)
                    AirForceGroups[(message.MapAreaId, message.GroupId)].Action = message.Action;
            });

            provider.AdmiralUpdated.Scan((Admiral)null!, (admiral, message) =>
            {
                if (admiral?.Id != message.Id)
                    return new Admiral(message, this);

                admiral.Update(message);
                return admiral;
            }).Subscribe(_admiralUpdated);

            var materialUpdate = Observable.Merge(new[]
            {
                provider.MaterialUpdated,
                instantConstructionUsed.Select(message => new InstantConstructionMaterialUpdate(message.IsLSC)),
            });
            materialUpdate.Scan(new Materials(), (materials, message) =>
            {
                message.Apply(materials);
                return materials;
            }).Subscribe(_materialsUpdated);
        }
    }
}

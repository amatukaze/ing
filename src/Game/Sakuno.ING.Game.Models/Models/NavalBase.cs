using Sakuno.ING.Game.Models.MasterData;
using System;

namespace Sakuno.ING.Game.Models
{
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

        private Admiral? _admiral;
        public Admiral Admiral => _admiral ?? throw new InvalidOperationException("Game not initialized");

        private Materials _materials;
        public Materials Materials
        {
            get => _materials;
            set => Set(ref _materials, value);
        }

        public NavalBase(GameProvider provider)
        {
            MasterData = new MasterDataRoot(provider);

            _constructionDocks = new IdTable<ConstructionDockId, ConstructionDock, RawConstructionDock, NavalBase>(this);
            _repairDocks = new IdTable<RepairDockId, RepairDock, RawRepairDock, NavalBase>(this);
            _useItems = new IdTable<UseItemId, UseItemCount, RawUseItemCount, NavalBase>(this);
            _slotItems = new IdTable<SlotItemId, PlayerSlotItem, RawSlotItem, NavalBase>(this);
            _ships = new IdTable<ShipId, PlayerShip, RawShip, NavalBase>(this);
            _fleets = new IdTable<FleetId, PlayerFleet, RawFleet, NavalBase>(this);

            provider.AdmiralUpdated.Subscribe(message =>
            {
                if (_admiral?.Id == message.Id)
                {
                    _admiral.Update(message);
                    return;
                }

                _admiral = new Admiral(message, this);
                NotifyPropertyChanged(nameof(Admiral));
            });

            provider.ConstructionDocksUpdated.Subscribe(message => _constructionDocks.BatchUpdate(message));
            provider.RepairDocksUpdate.Subscribe(message => _repairDocks.BatchUpdate(message));
            provider.UseItemsUpdated.Subscribe(message => _useItems.BatchUpdate(message));
            provider.SlotItemsUpdated.Subscribe(message => _slotItems.BatchUpdate(message));
            provider.ShipsUpdate.Subscribe(message => _ships.BatchUpdate(message));
            provider.FleetsUpdate.Subscribe(message => _fleets.BatchUpdate(message));

            provider.MaterialUpdate.Subscribe(message =>
            {
                var old = Materials;
                var materials = old;

                message.Apply(ref materials);

                if (old == materials)
                    return;

                Materials = materials;
            });
        }
    }
}

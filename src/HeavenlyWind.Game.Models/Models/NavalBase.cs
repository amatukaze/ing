using System.Collections.Generic;
using System.Linq;

namespace Sakuno.KanColle.Amatsukaze.Game.Models
{
    public class NavalBase : BindableObject, ITableProvider
    {
        public NavalBase(GameListener listener)
        {
            MasterData = new MasterDataRoot(listener);
            _allEquipment = new IdTable<Equipment, IRawEquipment>(this);
            _buildingDocks = new IdTable<BuildingDock, IRawBuildingDock>(this);
            _useItems = new IdTable<UseItemCount, IRawUseItemCount>(this);
            _allShips = new IdTable<Ship, IRawShip>(this);
            _fleets = new IdTable<Fleet, IRawFleet>(this);

            listener.AllEquipmentUpdated.Received += msg => _allEquipment.BatchUpdate(msg.Message);
            listener.BuildingDockUpdated.Received += msg => _buildingDocks.BatchUpdate(msg.Message);
            listener.UseItemUpdated.Received += msg => _useItems.BatchUpdate(msg.Message);
            listener.FreeEquipmentUpdated.Received += msg =>
            {
                var free = new HashSet<int>(msg.Message.SelectMany(x => x.Value));
                foreach (var e in AllEquipment)
                    e.IsAvailable = free.Contains(e.Id);
            };

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
            listener.HomeportUpdated.Received += msg =>
            {
                _allShips.BatchUpdate(msg.Message.Ships);
                _fleets.BatchUpdate(msg.Message.Fleets);
            };
        }

        public MasterDataRoot MasterData { get; }

        private readonly IdTable<Equipment, IRawEquipment> _allEquipment;
        public ITable<Equipment> AllEquipment => _allEquipment;

        private readonly IdTable<BuildingDock, IRawBuildingDock> _buildingDocks;
        public ITable<BuildingDock> BuildingDocks => _buildingDocks;

        private readonly IdTable<UseItemCount, IRawUseItemCount> _useItems;
        public ITable<UseItemCount> UseItems => _useItems;

        private readonly IdTable<Ship, IRawShip> _allShips;
        public ITable<Ship> AllShips => _allShips;

        private readonly IdTable<Fleet, IRawFleet> _fleets;
        public ITable<Fleet> Fleets => _fleets;

        public Admiral Admiral { get; private set; }

        private Materials _materials;
        public Materials Materials
        {
            get => _materials;
            set => Set(ref _materials, value);
        }

        public ITable<T> GetTable<T>()
        {
            var type = typeof(T);

            if (type == typeof(Equipment))
                return (ITable<T>)AllEquipment;

            if (type == typeof(BuildingDock))
                return (ITable<T>)BuildingDocks;

            if (type == typeof(UseItemCount))
                return (ITable<T>)UseItems;

            return MasterData.GetTable<T>();
        }
    }
}

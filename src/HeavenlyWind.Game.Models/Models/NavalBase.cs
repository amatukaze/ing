using System.Collections.Generic;
using System.Linq;

namespace Sakuno.KanColle.Amatsukaze.Game.Models
{
    public class NavalBase : ITableProvider
    {
        public NavalBase(GameListener listener)
        {
            MasterData = new MasterDataRoot(listener);
            _allEquipment = new IdTable<Equipment, IRawEquipment>(this);
            _buildingDocks = new IdTable<BuildingDock, IRawBuildingDock>(this);
            _useItems = new IdTable<UseItemCount, IRawUseItemCount>(this);

            listener.AllEquipmentUpdated.Received += msg => _allEquipment.BatchUpdate(msg.Message);
            listener.BuildingDockUpdated.Received += msg => _buildingDocks.BatchUpdate(msg.Message);
            listener.UseItemUpdated.Received += msg => _useItems.BatchUpdate(msg.Message);
            listener.FreeEquipmentUpdated.Received += msg =>
            {
                var free = new HashSet<int>(msg.Message.SelectMany(x => x.Value));
                foreach (var e in AllEquipment)
                    e.IsAvailable = free.Contains(e.Id);
            };
        }

        public MasterDataRoot MasterData { get; }

        private readonly IdTable<Equipment, IRawEquipment> _allEquipment;
        public ITable<Equipment> AllEquipment => _allEquipment;

        private readonly IdTable<BuildingDock, IRawBuildingDock> _buildingDocks;
        public ITable<BuildingDock> BuildingDocks => _buildingDocks;

        private readonly IdTable<UseItemCount, IRawUseItemCount> _useItems;
        public ITable<UseItemCount> UseItems => _useItems;

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

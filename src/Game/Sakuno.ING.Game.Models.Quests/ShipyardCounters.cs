using System;
using System.Collections.Generic;
using System.Linq;
using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Models.Quests
{
    internal class ShipPowerupCounter : QuestCounter
    {
        private readonly Func<HomeportShip, IReadOnlyCollection<HomeportShip>, bool> powerupFilter;

        public ShipPowerupCounter(QuestId questId, int maximum, Func<HomeportShip, IReadOnlyCollection<HomeportShip>, bool> powerupFilter = null, int counterId = 0)
            : base(questId, maximum, counterId)
        {
            this.powerupFilter = powerupFilter;
        }

        public void OnShipPowerup(IStatePersist statePersist, HomeportShip ship, IReadOnlyCollection<HomeportShip> consumed, bool success)
        {
            if (success && powerupFilter?.Invoke(ship, consumed) != false)
                Increase(statePersist);
        }
    }

    internal class SingletonEventCounter : QuestCounter
    {
        private readonly SingletonEvent @event;

        public SingletonEventCounter(QuestId questId, int maximum, SingletonEvent @event, int counterId = 0) : base(questId, maximum, counterId)
        {
            this.@event = @event;
        }

        public void OnSingletonEvent(IStatePersist statePersist, SingletonEvent @event)
        {
            if (this.@event == @event)
                Increase(statePersist);
        }
    }

    internal class ShipDismantleCounter : QuestCounter
    {
        public ShipDismantleCounter(QuestId questId, int maximum, int counterId = 0) : base(questId, maximum, counterId)
        {
        }

        public void OnShipDismantle(IStatePersist statePersist, IReadOnlyCollection<HomeportShip> ships)
            => Increase(statePersist, ships.Count);
    }

    internal class EquipmentDismantleCounter : QuestCounter
    {
        public EquipmentDismantleCounter(QuestId questId, int maximum, int counterId = 0) : base(questId, maximum, counterId)
        {
        }

        public void OnEquipmentDismantle(IStatePersist statePersist, IReadOnlyCollection<HomeportEquipment> equipment)
            => Increase(statePersist, IncreaseCount(equipment));

        protected virtual int IncreaseCount(IReadOnlyCollection<HomeportEquipment> equipment) => 1;
    }

    internal class EquipmentDismantleTypedCounter : EquipmentDismantleCounter
    {
        private readonly Predicate<EquipmentInfo> equipmentFilter;

        public EquipmentDismantleTypedCounter(QuestId questId, int maximum, int counterId = 0, Predicate<EquipmentInfo> equipmentFilter = null)
            : base(questId, maximum, counterId)
        {
            this.equipmentFilter = equipmentFilter;
        }

        public EquipmentDismantleTypedCounter(QuestId questId, int maximum, EquipmentInfoId id, int counterId = 0)
            : base(questId, maximum, counterId)
        {
            equipmentFilter = e => e.Id == id;
        }

        public EquipmentDismantleTypedCounter(QuestId questId, int maximum, EquipmentTypeId typeId, int counterId = 0)
            : base(questId, maximum, counterId)
        {
            equipmentFilter = e => e.Type?.Id == typeId;
        }

        protected override int IncreaseCount(IReadOnlyCollection<HomeportEquipment> equipment)
            => equipment.Count(x => equipmentFilter?.Invoke(x.Info) != false);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Models.Quests
{
    internal class ShipPowerupCounter : QuestCounter
    {
        private readonly Func<HomeportShip, IReadOnlyCollection<HomeportShip>, bool> powerupFilter;

        public ShipPowerupCounter(in QuestCounterParams @params, Func<HomeportShip, IReadOnlyCollection<HomeportShip>, bool> powerupFilter = null)
            : base(@params)
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

        public SingletonEventCounter(in QuestCounterParams @params, SingletonEvent @event) : base(@params)
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
        public ShipDismantleCounter(in QuestCounterParams @params) : base(@params)
        {
        }

        public void OnShipDismantle(IStatePersist statePersist, IReadOnlyCollection<HomeportShip> ships)
            => Increase(statePersist, ships.Count);
    }

    internal class EquipmentDismantleCounter : QuestCounter
    {
        public EquipmentDismantleCounter(in QuestCounterParams @params) : base(@params)
        {
        }

        public void OnEquipmentDismantle(IStatePersist statePersist, IReadOnlyCollection<HomeportEquipment> equipment)
            => Increase(statePersist, IncreaseCount(equipment));

        protected virtual int IncreaseCount(IReadOnlyCollection<HomeportEquipment> equipment) => 1;
    }

    internal class EquipmentDismantleTypedCounter : EquipmentDismantleCounter
    {
        private readonly Predicate<EquipmentInfo> equipmentFilter;

        public EquipmentDismantleTypedCounter(in QuestCounterParams @params, Predicate<EquipmentInfo> equipmentFilter = null)
            : base(@params)
        {
            this.equipmentFilter = equipmentFilter;
        }

        protected override int IncreaseCount(IReadOnlyCollection<HomeportEquipment> equipment)
            => equipment.Count(x => equipmentFilter?.Invoke(x.Info) != false);
    }
}

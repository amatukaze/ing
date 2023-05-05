using System;
using System.Collections.Generic;
using System.Linq;

namespace Sakuno.ING.Game.Models
{
    public partial class Ship
    {
        public abstract IBindableCollection<Slot> Slots { get; }
        public abstract Slot ExtraSlot { get; }

        public IEnumerable<Slot> AllSlots => ExtraSlot is null ? Slots : Slots.Append(ExtraSlot);

        internal event Action CalculationUpdated;
        protected void DoCalculations()
        {
            using (EnterBatchNotifyScope())
            {
                bool isMarriaged = Leveling.Level >= 100;
                SupplyingCost = new Materials
                {
                    Fuel = isMarriaged ? (int)(Fuel.Shortage * 0.85) : Fuel.Shortage,
                    Bullet = isMarriaged ? (int)(Bullet.Shortage * 0.85) : Bullet.Shortage,
                    Bauxite = Slots.Sum(x => x.Aircraft.Shortage) * 5
                };
                AirFightPower = Slots.Sum(x => x.AirFightPower);
                EffectiveLoS = new LineOfSight(AllSlots.Sum(x => x.EffectiveLoS), Math.Sqrt(LineOfSight.Current));
            }
            CalculationUpdated?.Invoke();
        }
    }
}

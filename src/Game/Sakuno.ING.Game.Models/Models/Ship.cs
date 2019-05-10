using System;
using System.Linq;

namespace Sakuno.ING.Game.Models
{
    public partial class Ship
    {
        public abstract IBindableCollection<Slot> Slots { get; }
        public abstract Slot ExtraSlot { get; }

        internal event Action CalculationUpdated;
        protected void DoCalculations()
        {
            using (EnterBatchNotifyScope())
            {
                SupplyingCost = new Materials
                {
                    Fuel = Fuel.Shortage,
                    Bullet = Bullet.Shortage,
                    Bauxite = Slots.Sum(x => x.Aircraft.Shortage) * 5
                };
                AirFightPower = Slots.Sum(x => x.AirFightPower);
                EffectiveLoS = new LineOfSight(Slots.Sum(x => x.EffectiveLoS), Math.Sqrt(LineOfSight.Current));
            }
            CalculationUpdated?.Invoke();
        }
    }
}

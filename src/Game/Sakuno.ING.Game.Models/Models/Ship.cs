using System;
using System.Linq;

namespace Sakuno.ING.Game.Models
{
    public partial class Ship
    {
        protected BindableCollection<Slot> slots = new BindableCollection<Slot>();
        public IBindableCollection<Slot> Slots => slots;

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
        }
    }
}

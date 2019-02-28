using System;
using System.Linq;

namespace Sakuno.ING.Game.Models
{
    public partial class Ship
    {
        public abstract IBindableCollection<Slot> Equipment { get; }

        protected void DoCalculations()
        {
            using (EnterBatchNotifyScope())
            {
                SupplyingCost = new Materials
                {
                    Fuel = Fuel.Shortage,
                    Bullet = Bullet.Shortage,
                    Bauxite = Equipment.Sum(x => x.Aircraft.Shortage) * 5
                };
                AirFightPower = Equipment.Sum(x => x.AirFightPower);
                EffectiveLoS = new LineOfSight(Equipment.Sum(x => x.EffectiveLoS), Math.Sqrt(LineOfSight.Current));
            }
        }
    }
}

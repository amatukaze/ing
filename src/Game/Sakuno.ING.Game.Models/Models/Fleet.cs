using System.Collections.Generic;
using System.Linq;

namespace Sakuno.ING.Game.Models
{
    public partial class Fleet
    {
        protected abstract int AdmiralLevel { get; }
        public abstract IBindableCollection<Ship> Ships { get; }

        protected void DoCalculations()
        {
            SlowestShipSpeed = Ships.AsList().Count > 0 ? Ships.Min(s => s.Speed) : ShipSpeed.None;
            SupplyingCost = Ships.Sum(s => s.SupplyingCost);
            RepairingCost = Ships.Sum(s => s.RepairingCost);
            AirFightPower = Ships.Sum(s => s.AirFightPower);
            SimpleLoS = Ships.Sum(s => s.LineOfSight.Displaying);
            var sumLoS = Ships.Sum(s => s.EffectiveLoS);
            EffectiveLoS = new LineOfSight(sumLoS.Multiplied, sumLoS.Baseline + 2 * (6 - Ships.AsList().Count), AdmiralLevel);
        }
    }

    public class ImplicitFleet : Fleet
    {
        public ImplicitFleet(IEnumerable<Ship> ships, int admiralLevel = 0)
        {
            Ships = ships.ToBindable();
            AdmiralLevel = admiralLevel;
        }

        public override IBindableCollection<Ship> Ships { get; }
        protected override int AdmiralLevel { get; }
    }
}

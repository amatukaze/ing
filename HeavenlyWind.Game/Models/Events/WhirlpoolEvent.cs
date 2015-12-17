using Sakuno.KanColle.Amatsukaze.Game.Models.Raw;
using System.Linq;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.Events
{
    public class WhirlpoolEvent : SortieEvent
    {
        public MaterialType LostMaterial { get; }
        public int Amount { get; }

        public bool HasReduceLossesWithRadar { get; }

        public string Name { get; }

        internal WhirlpoolEvent(RawMapExploration rpData) : base(rpData)
        {
            LostMaterial = rpData.Whirlpool.MaterialType;
            Amount = rpData.Whirlpool.Amount;

            if (LostMaterial == MaterialType.Fuel)
                Name = StringResources.Instance.Main.Material_Fuel;
            else
                Name = StringResources.Instance.Main.Material_Bullet;

            var rShips = KanColleGame.Current.Port.Fleets.Table.Values
                .Where(r => (r.State & FleetState.Sortie) == FleetState.Sortie)
                .SelectMany(r => r.Ships);
            var rMaxAmount = (double)rShips.Max(r => LostMaterial == MaterialType.Fuel ? r.Fuel.Current : r.Bullet.Current);
            var rReducedRate = Amount / rMaxAmount;

            foreach (var rShip in rShips)
                if (LostMaterial == MaterialType.Fuel)
                    rShip.Fuel = rShip.Fuel.Update(rShip.Fuel.Current - (int)(rShip.Fuel.Current * rReducedRate));
                else
                    rShip.Bullet = rShip.Bullet.Update(rShip.Bullet.Current - (int)(rShip.Bullet.Current * rReducedRate));
        }
    }
}

using Sakuno.KanColle.Amatsukaze.Game.Models.Raw;
using System.Collections.Generic;
using System.Linq;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.Events
{
    public class WhirlpoolEvent : SortieEvent
    {
        public MaterialType LostMaterial => RawData.Whirlpool.MaterialType;
        public int Amount => RawData.Whirlpool.Amount;

        public bool HasReduceLossesWithRadar => RawData.Whirlpool.HasReduceLossesWithRadar;

        internal WhirlpoolEvent(RawMapExploration rpData) : base(rpData)
        {
            var rSortie = SortieInfo.Current;
            IEnumerable<Ship> rShips = rSortie.Fleet.Ships;
            if (rSortie.EscortFleet != null)
                rShips = rShips.Concat(rSortie.EscortFleet.Ships);

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

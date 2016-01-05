using Sakuno.KanColle.Amatsukaze.Game.Models.Raw;
using System.Linq;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.Events
{
    public class WhirlpoolEvent : SortieEvent
    {
        public MaterialType LostMaterial => RawData.Whirlpool.MaterialType;
        public int Amount => RawData.Whirlpool.Amount;

        public bool HasReduceLossesWithRadar => RawData.Whirlpool.HasReduceLossesWithRadar;

        public string Name => LostMaterial == MaterialType.Fuel ? StringResources.Instance.Main.Material_Fuel : StringResources.Instance.Main.Material_Bullet;

        internal WhirlpoolEvent(RawMapExploration rpData) : base(rpData)
        {
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

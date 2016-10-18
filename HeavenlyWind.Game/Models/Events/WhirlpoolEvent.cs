using Sakuno.KanColle.Amatsukaze.Game.Models.Raw;
using System.Collections.Generic;
using System.Linq;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.Events
{
    public class WhirlpoolEvent : SortieEvent
    {
        public MaterialType LostMaterial => RawData.Whirlpool.MaterialType;
        public int Amount => RawData.Whirlpool.Amount;

        public string Message { get; }

        internal WhirlpoolEvent(IEnumerable<Ship> rpShips, Fleet rpEscortFleet, RawMapExploration rpData) : base(rpData)
        {
            var rMaxAmount = (double)rpShips.Max(r => LostMaterial == MaterialType.Fuel ? r.Fuel.Current : r.Bullet.Current);
            var rReducedRate = Amount / rMaxAmount;

            var rTotalAmount = 0;

            foreach (var rShip in rpShips)
            {
                int rReducedAmount;

                if (LostMaterial == MaterialType.Fuel)
                {
                    rReducedAmount = (int)(rShip.Fuel.Current * rReducedRate);
                    rShip.Fuel.Current -= rReducedAmount;
                }
                else
                {
                    rReducedAmount = (int)(rShip.Bullet.Current * rReducedRate);
                    rShip.Bullet.Current -= rReducedAmount;
                }

                rTotalAmount += rReducedAmount;
            }

            var rMessage = rpEscortFleet == null ? StringResources.Instance.Main.Sortie_Whirlpool_Message : StringResources.Instance.Main.Sortie_Whirlpool_Message_CombinedFleet;
            Message = string.Format(rMessage, LostMaterial == MaterialType.Fuel ? "[icon]fuel[/icon]" : "[icon]bullet[/icon]", rTotalAmount, rReducedRate);
        }
    }
}

using Sakuno.KanColle.Amatsukaze.Game.Models;

namespace Sakuno.KanColle.Amatsukaze.ViewModels.Overviews
{
    public class ShipViewModel
    {
        public int No { get; }
        public Ship Ship { get; }

        public ShipTypeViewModel Type { get; }

        public ShipViewModel(int rpNo, Ship rpShip, ShipTypeViewModel rpType)
        {
            No = rpNo;
            Ship = rpShip;

            Type = rpType;
        }
    }
}

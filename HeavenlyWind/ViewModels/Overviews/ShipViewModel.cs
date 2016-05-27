using Sakuno.KanColle.Amatsukaze.Game.Models;

namespace Sakuno.KanColle.Amatsukaze.ViewModels.Overviews
{
    public class ShipViewModel : ModelBase
    {
        public Ship Ship { get; }
        public ShipTypeViewModel Type { get; }

        public ShipViewModel(Ship rpShip, ShipTypeViewModel rpType)
        {
            Ship = rpShip;
            Type = rpType;
        }
    }
}

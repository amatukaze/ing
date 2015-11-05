using Sakuno.KanColle.Amatsukaze.Game.Models;

namespace Sakuno.KanColle.Amatsukaze.ViewModels.Game
{
    public class ShipViewModel : ModelBase
    {
        public Ship Source { get; }

        internal ShipViewModel(Ship rpShip)
        {
            Source = rpShip;
        }
    }
}

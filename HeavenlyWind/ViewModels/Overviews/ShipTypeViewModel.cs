using Sakuno.KanColle.Amatsukaze.Game.Models;

namespace Sakuno.KanColle.Amatsukaze.ViewModels.Overviews
{
    public class ShipTypeViewModel : FilterTypeViewModel
    {
        public int ID { get; }

        internal ShipTypeViewModel(ShipType rpShipType)
        {
            ID = rpShipType.ID;
            Name = rpShipType.Name;
        }
    }
}

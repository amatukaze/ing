using Sakuno.KanColle.Amatsukaze.Game.Models;

namespace Sakuno.KanColle.Amatsukaze.ViewModels.Overviews.Equipments
{
    public class EquipmentsGroupByShip
    {
        public Ship Ship { get; }
        public int Count { get; internal set; }

        public string CountDisplayString => Count == 1 ? string.Empty : " x " + Count;

        internal EquipmentsGroupByShip(Ship rpShip)
        {
            Ship = rpShip;
        }
    }
}

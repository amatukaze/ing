using Sakuno.KanColle.Amatsukaze.Game.Models;

namespace Sakuno.KanColle.Amatsukaze.ViewModels.Overviews
{
    public class EquipmentGroupByShip
    {
        public Ship Ship { get; }
        public int Count { get; internal set; }

        public string CountDisplayString => Count == 1 ? string.Empty : " x " + Count;

        internal EquipmentGroupByShip(Ship rpShip)
        {
            Ship = rpShip;
        }
    }
}

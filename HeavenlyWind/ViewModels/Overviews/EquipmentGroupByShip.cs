using Sakuno.KanColle.Amatsukaze.Game.Models;

namespace Sakuno.KanColle.Amatsukaze.ViewModels.Overviews
{
    class EquipmentGroupByShip : ModelBase
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

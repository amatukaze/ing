using Sakuno.KanColle.Amatsukaze.Game.Models;

namespace Sakuno.KanColle.Amatsukaze.ViewModels.Overviews
{
    class EquipmentGroupByShip : ModelBase
    {
        public Ship Ship { get; }
        public int Count { get; internal set; }

        internal EquipmentGroupByShip(Ship rpShip)
        {
            Ship = rpShip;
        }
    }
}

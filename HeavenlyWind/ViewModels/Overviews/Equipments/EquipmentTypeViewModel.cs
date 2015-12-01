using Sakuno.KanColle.Amatsukaze.Game.Models;

namespace Sakuno.KanColle.Amatsukaze.ViewModels.Overviews.Equipments
{
    public class EquipmentTypeViewModel : FilterTypeViewModel
    {
        public EquipmentIconType Type { get; }

        public EquipmentTypeViewModel(EquipmentIconType rpType)
        {
            Type = rpType;
        }
    }
}

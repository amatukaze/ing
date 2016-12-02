using Sakuno.KanColle.Amatsukaze.Game.Models;
using System;
using System.Windows.Input;

namespace Sakuno.KanColle.Amatsukaze.ViewModels.Overviews
{
    public class EquipmentTypeViewModel : FilterTypeViewModel
    {
        public EquipmentIconType Type { get; }

        public ICommand SelectThisTypeOnlyCommand { get; internal set; }

        public EquipmentTypeViewModel(EquipmentIconType rpType)
        {
            Type = rpType;
        }
    }
}

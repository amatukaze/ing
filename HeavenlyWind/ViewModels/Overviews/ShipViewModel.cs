using Sakuno.KanColle.Amatsukaze.Game.Models;
using System.Collections.Generic;

namespace Sakuno.KanColle.Amatsukaze.ViewModels.Overviews
{
    class ShipViewModel : ModelBase, IID
    {
        public Ship Ship { get; }

        public FilterTypeViewModel<ShipTypeInfo> Type { get; private set; }

        int IID.ID => Ship.ID;

        public ShipViewModel(Ship rpShip, FilterTypeViewModel<ShipTypeInfo> rpType)
        {
            Ship = rpShip;
            Type = rpType;
        }

        public void UpdateType(IDictionary<ShipTypeInfo, FilterTypeViewModel<ShipTypeInfo>> rpTypes)
        {
            if (Ship.Info.Type.ID == Type.Type.ID)
                return;

            Type = rpTypes[Ship.Info.Type];
            OnPropertyChanged(nameof(Type));
        }
    }
}

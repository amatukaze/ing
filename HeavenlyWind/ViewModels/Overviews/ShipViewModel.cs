using System;
using Sakuno.KanColle.Amatsukaze.Game.Models;
using System.Collections.Generic;

namespace Sakuno.KanColle.Amatsukaze.ViewModels.Overviews
{
    class ShipViewModel : ModelBase, IID
    {
        public Ship Ship { get; }

        public ShipTypeViewModel Type { get; private set; }

        int IID.ID => Ship.ID;

        public ShipViewModel(Ship rpShip, ShipTypeViewModel rpType)
        {
            Ship = rpShip;
            Type = rpType;
        }

        public void UpdateType(IDictionary<ShipTypeInfo, ShipTypeViewModel> rpTypes)
        {
            if (Ship.Info.Type.ID != Type.ID)
            {
                Type = rpTypes[Ship.Info.Type];
                OnPropertyChanged(nameof(Type));
            }
        }
    }
}

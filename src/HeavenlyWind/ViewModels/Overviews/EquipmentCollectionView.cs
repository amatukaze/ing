using Sakuno.KanColle.Amatsukaze.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sakuno.KanColle.Amatsukaze.ViewModels.Overviews
{
    class EquipmentCollectionView : ItemsView<ShipViewModel>
    {
        protected override IEnumerable<ShipViewModel> Source
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        protected override bool Filter(ShipViewModel rpItem)
        {
            throw new NotImplementedException();
        }
    }
}

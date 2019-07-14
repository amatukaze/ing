using Microsoft.Toolkit.Uwp.UI;
using Sakuno.ING.Game.Models;
using Sakuno.ING.Shell;
using Windows.UI.Xaml.Controls;

namespace Sakuno.ING.Views.UWP.Catalog
{
    [ExportView("ShipCatalog")]
    public sealed partial class ShipCatalogView : UserControl
    {
        private readonly AdvancedCollectionView AllShipsView;

        public ShipCatalogView(NavalBase navalBase)
        {
            AllShipsView = new AdvancedCollectionView(navalBase.AllShips.DefaultView, isLiveShaping: true);
            InitializeComponent();
        }
    }
}

using Sakuno.ING.Shell;
using Sakuno.ING.ViewModels.Catalog;
using Windows.UI.Xaml.Controls;

namespace Sakuno.ING.Views.UWP.Catalog
{
    [ExportView("EquipmentCatalog")]
    public sealed partial class EquipmentCatalogView : UserControl
    {
        private readonly EquipmentCatalogVM ViewModel;

        public EquipmentCatalogView(EquipmentCatalogVM viewModel)
        {
            ViewModel = viewModel;
            InitializeComponent();
        }
    }
}

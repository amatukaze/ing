using Sakuno.ING.Shell;
using Sakuno.ING.ViewModels.Logging;
using Windows.UI.Xaml.Controls;

namespace Sakuno.ING.Views.UWP.Logging
{
    [ExportView("ShipCreationLogs")]
    public sealed partial class ShipCreationLogsView : UserControl
    {
        private ShipCreationLogsVM ViewModel;
        public ShipCreationLogsView(ShipCreationLogsVM viewModel)
        {
            ViewModel = viewModel;
            this.InitializeComponent();
        }
    }
}

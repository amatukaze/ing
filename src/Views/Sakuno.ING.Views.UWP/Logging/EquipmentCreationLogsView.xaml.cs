using Sakuno.ING.Shell;
using Sakuno.ING.ViewModels.Logging;
using Windows.UI.Xaml.Controls;

namespace Sakuno.ING.Views.UWP.Logging
{
    [ExportView("EquipmentCreationLogs")]
    public sealed partial class EquipmentCreationLogsView : UserControl
    {
        private readonly EquipmentCreationLogsVM ViewModel;
        public EquipmentCreationLogsView(EquipmentCreationLogsVM viewModel)
        {
            ViewModel = viewModel;
            this.InitializeComponent();
        }
    }
}

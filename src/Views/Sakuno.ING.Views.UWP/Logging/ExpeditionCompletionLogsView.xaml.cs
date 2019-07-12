using Sakuno.ING.Shell;
using Sakuno.ING.ViewModels.Logging;
using Windows.UI.Xaml.Controls;

namespace Sakuno.ING.Views.UWP.Logging
{
    [ExportView("ExpeditionCompletionLogs")]
    public sealed partial class ExpeditionCompletionLogsView : UserControl
    {
        private readonly ExpeditionCompletionLogsVM ViewModel;
        public ExpeditionCompletionLogsView(ExpeditionCompletionLogsVM viewModel)
        {
            ViewModel = viewModel;
            InitializeComponent();
        }
    }
}

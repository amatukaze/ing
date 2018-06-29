using Sakuno.ING.Shell;
using Sakuno.ING.ViewModels.Logging;
using Windows.UI.Xaml.Controls;

namespace Sakuno.ING.Views.UWP.Logging
{
    [ExportView("LogMigration")]
    public sealed partial class LogMigrationView : UserControl
    {
        private LogMigrationVM ViewModel;
        public LogMigrationView(LogMigrationVM viewModel)
        {
            ViewModel = viewModel;
            this.InitializeComponent();
        }
    }
}

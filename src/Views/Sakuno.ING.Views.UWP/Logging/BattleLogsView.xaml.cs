using Sakuno.ING.Shell;
using Sakuno.ING.ViewModels.Logging;
using Windows.UI.Xaml.Controls;

namespace Sakuno.ING.Views.UWP.Logging
{
    [ExportView("BattleLogs")]
    public sealed partial class BattleLogsView : UserControl
    {
        private readonly BattleLogsVM ViewModel;
        public BattleLogsView(BattleLogsVM viewModel)
        {
            ViewModel = viewModel;
            InitializeComponent();
            Unloaded += (s, e) => ViewModel.Dispose();
        }
    }
}

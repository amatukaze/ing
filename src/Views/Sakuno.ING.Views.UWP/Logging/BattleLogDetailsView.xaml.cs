using Sakuno.ING.Shell;
using Sakuno.ING.ViewModels.Logging;
using Windows.UI.Xaml.Controls;

namespace Sakuno.ING.Views.UWP.Logging
{
    [ExportView("BattleLogDetail")]
    public sealed partial class BattleLogDetailsView : UserControl
    {
        private readonly BattleVM ViewModel;

        public BattleLogDetailsView(BattleVM viewModel)
        {
            ViewModel = viewModel;
            InitializeComponent();
        }
    }
}

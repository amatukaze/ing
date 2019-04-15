using Sakuno.ING.Game.Models.Combat;
using Sakuno.ING.Shell;
using Sakuno.ING.ViewModels.Combat;
using Windows.UI.Xaml.Controls;

namespace Sakuno.ING.Views.UWP.Combat
{
    [ExportView("CurrentBattle")]
    public sealed partial class CurrentBattleView : UserControl
    {
        private readonly CurrentBattleVM ViewModel;
        private readonly BattleManager Manager;

        public CurrentBattleView(CurrentBattleVM viewModel)
        {
            ViewModel = viewModel;
            Manager = viewModel.BattleManager;
            this.InitializeComponent();
        }
    }
}

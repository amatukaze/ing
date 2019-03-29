using Sakuno.ING.Game.Models;
using Sakuno.ING.Game.Models.Combat;
using Sakuno.ING.Shell;
using Windows.UI.Xaml.Controls;

namespace Sakuno.ING.Views.UWP.Combat
{
    [ExportView("CurrentBattle")]
    public sealed partial class CurrentBattleView : UserControl
    {
        private readonly BattleManager Manager;

        public CurrentBattleView(NavalBase navalBase)
        {
            Manager = navalBase.Battle;
            this.InitializeComponent();
        }
    }
}

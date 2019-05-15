using Sakuno.ING.Composition;
using Sakuno.ING.Game.Models;
using Sakuno.ING.Game.Models.Combat;
using Sakuno.ING.Shell;

namespace Sakuno.ING.ViewModels.Combat
{
    [Export(typeof(CurrentBattleVM))]
    public class CurrentBattleVM
    {
        public BattleManager BattleManager { get; }
        private readonly IShell shell;

        public CurrentBattleVM(NavalBase navalBase, IShell shell)
        {
            BattleManager = navalBase.Battle;
            this.shell = shell;
        }

        public void ShowCurrentDetail()
        {
            if (BattleManager.CurrentBattle != null)
                shell.ShowViewWithParameter("CurrentBattleDetail", BattleManager.CurrentBattle);
        }

        public void ShowCurrentLandBaseDefence()
        {
            if (BattleManager.CurrentRouting?.LandBaseDefence != null)
                shell.ShowViewWithParameter("LandBaseDefenceDetail", BattleManager.CurrentRouting.LandBaseDefence);
        }
    }
}

using System.Windows.Controls;
using Sakuno.ING.Game.Models.Combat;
using Sakuno.ING.Shell;

namespace Sakuno.ING.Views.Desktop.Combat
{
    [ExportView("LandBaseDefenceDetail")]
    public partial class BattleOverview : UserControl
    {
        public BattleOverview()
        {
            InitializeComponent();
        }

        public BattleOverview(BattleBase battle)
            : this()
        {
            DataContext = battle;
        }
    }
}

using System.Windows.Controls;
using Sakuno.ING.Game.Models.Combat;
using Sakuno.ING.Shell;

namespace Sakuno.ING.Views.Desktop.Combat
{
    [ExportView("CurrentBattleDetail")]
    public partial class BattleDetailView : UserControl
    {
        public BattleDetailView()
        {
            InitializeComponent();
        }

        public BattleDetailView(BattleBase battle)
            : this()
        {
            DataContext = battle;
        }
    }
}

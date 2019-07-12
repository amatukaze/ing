using Sakuno.ING.Game.Models.Combat;
using Sakuno.ING.Shell;
using Windows.UI.Xaml.Controls;

namespace Sakuno.ING.Views.UWP.Combat
{
    [ExportView("LandBaseDefenceDetail")]
    public sealed partial class BattleOverview : UserControl
    {
        private BattleBase _battle;
        public BattleBase Battle
        {
            get => _battle;
            set
            {
                _battle = value;
                Bindings.Update();
            }
        }

        public BattleOverview()
        {
            InitializeComponent();
        }

        public BattleOverview(BattleBase battle)
        {
            _battle = battle;
            InitializeComponent();
        }
    }
}

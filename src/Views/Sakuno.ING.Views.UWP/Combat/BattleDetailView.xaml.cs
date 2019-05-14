using Sakuno.ING.Game.Models.Combat;
using Sakuno.ING.Shell;
using Windows.UI.Text;
using Windows.UI.Xaml.Controls;

namespace Sakuno.ING.Views.UWP.Combat
{
    [ExportView("CurrentBattleDetail")]
    public sealed partial class BattleDetailView : UserControl
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

        public BattleDetailView()
        {
            this.InitializeComponent();
        }

        public BattleDetailView(BattleBase battle)
        {
            _battle = battle;
            this.InitializeComponent();
        }

        public static FontWeight BooleanToFontWeight(bool isCritial)
            => isCritial ? FontWeights.Bold : FontWeights.Normal;
    }
}

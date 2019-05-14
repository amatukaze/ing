using Sakuno.ING.Game.Models.Combat;
using Windows.UI.Xaml.Controls;

namespace Sakuno.ING.Views.UWP.Combat
{
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
            this.InitializeComponent();
        }
    }
}

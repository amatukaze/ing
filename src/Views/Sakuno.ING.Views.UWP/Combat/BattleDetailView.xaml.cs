using Sakuno.ING.Game.Models.Combat;
using Windows.UI.Text;
using Windows.UI.Xaml.Controls;

namespace Sakuno.ING.Views.UWP.Combat
{
    public sealed partial class BattleDetailView : UserControl
    {
        private Battle _battle;
        public Battle Battle
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

        public static FontWeight BooleanToFontWeight(bool isCritial)
            => isCritial ? FontWeights.Bold : FontWeights.Normal;
    }
}

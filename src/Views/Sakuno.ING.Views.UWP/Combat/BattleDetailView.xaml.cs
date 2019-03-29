using Sakuno.ING.Game.Models.Combat;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Sakuno.ING.Views.UWP.Combat
{
    public sealed partial class BattleDetailView : UserControl
    {
        public BattleDetailView()
        {
            this.InitializeComponent();
        }

        public static readonly DependencyProperty BattleProperty
            = DependencyProperty.Register(nameof(Battle), typeof(Battle), typeof(BattleDetailView), new PropertyMetadata(null));
        public Battle Battle
        {
            get => (Battle)GetValue(BattleProperty);
            set => SetValue(BattleProperty, value);
        }
    }
}

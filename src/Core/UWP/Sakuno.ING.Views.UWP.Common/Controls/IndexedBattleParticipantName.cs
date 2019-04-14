using Sakuno.ING.Game.Models.Combat;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Sakuno.ING.Views.UWP.Controls
{
    public sealed class IndexedBattleParticipantName : Control
    {
        public IndexedBattleParticipantName()
        {
            DefaultStyleKey = typeof(IndexedBattleParticipantName);
        }

        public static readonly DependencyProperty SourceProperty
            = DependencyProperty.Register(nameof(Participant), typeof(BattleParticipant), typeof(IndexedBattleParticipantName),
                new PropertyMetadata(null, (d, e) => ((IndexedBattleParticipantName)d).CheckVisualState()));

        public BattleParticipant Participant
        {
            get => (BattleParticipant)GetValue(SourceProperty);
            set => SetValue(SourceProperty, value);
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            CheckVisualState();
        }

        private void CheckVisualState() => VisualStateManager.GoToState(this, Participant?.IsEnemy switch
        {
            true => "Enemy",
            false => "Ally",
            null => "Unknown"
        }, true);
    }
}

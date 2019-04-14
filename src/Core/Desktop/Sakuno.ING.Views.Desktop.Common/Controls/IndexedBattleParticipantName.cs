using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Sakuno.ING.Game.Models.Combat;

namespace Sakuno.ING.Views.Desktop.Controls
{
    public class IndexedBattleParticipantName : Control
    {
        static IndexedBattleParticipantName()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(IndexedBattleParticipantName), new FrameworkPropertyMetadata(typeof(IndexedBattleParticipantName)));
        }

        public static readonly DependencyProperty ParticipantProperty
            = DependencyProperty.Register(nameof(Participant), typeof(BattleParticipant), typeof(IndexedBattleParticipantName),
                new PropertyMetadata(null));
        public BattleParticipant Participant
        {
            get => (BattleParticipant)GetValue(ParticipantProperty);
            set => SetValue(ParticipantProperty, value);
        }

        public static readonly DependencyProperty IsEnemyProperty
            = DependencyProperty.Register(nameof(IsEnemy), typeof(bool?), typeof(IndexedBattleParticipantName),
                new PropertyMetadata(null));
        public bool? IsEnemy
        {
            get => (bool?)GetValue(IsEnemyProperty);
            set => SetValue(IsEnemyProperty, value);
        }

        public IndexedBattleParticipantName()
        {
            BindingOperations.SetBinding(this, IsEnemyProperty, new Binding
            {
                Source = this,
                Path = new PropertyPath($"{nameof(Participant)}.{nameof(BattleParticipant.IsEnemy)}")
            });
        }
    }
}

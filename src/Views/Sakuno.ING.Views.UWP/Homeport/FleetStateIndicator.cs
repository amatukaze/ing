using System;
using Sakuno.ING.Game.Models;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Shapes;

namespace Sakuno.ING.Views.UWP.Homeport
{
    [TemplatePart(Name = nameof(PART_Shape), Type = typeof(Shape))]
    internal class FleetStateIndicator : Control
    {
        public static readonly DependencyProperty StateProperty
            = DependencyProperty.Register(nameof(State), typeof(FleetState), typeof(FleetStateIndicator),
                new PropertyMetadata(FleetState.Empty, (d, e) => ((FleetStateIndicator)d).CheckState()));
        public FleetState State
        {
            get => (FleetState)GetValue(StateProperty);
            set => SetValue(StateProperty, value);
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            PART_Shape = GetTemplateChild(nameof(PART_Shape)) as Shape;
            if (PART_Shape != null)
            {
                PART_Shape.Fill = brush = new SolidColorBrush();
                var animation = new DoubleAnimation
                {
                    From = 1,
                    To = 0,
                    Duration = new Duration(TimeSpan.FromSeconds(0.5)),
                    AutoReverse = true,
                    RepeatBehavior = RepeatBehavior.Forever
                };
                Storyboard.SetTarget(animation, PART_Shape);
                Storyboard.SetTargetProperty(animation, nameof(Opacity));
                storyBoard = new Storyboard
                {
                    Children = { animation }
                };
            }
            else
            {
                brush = null;
                storyBoard = null;
            }
            CheckState();
        }

        private Shape PART_Shape;
        private SolidColorBrush brush;
        private Storyboard storyBoard;
        private void CheckState()
        {
            if (PART_Shape is null) return;
            var state = State;

            if (state == FleetState.Warning)
                storyBoard.Begin();
            else
                storyBoard.Stop();

            brush.Color = state switch
            {
                FleetState.Empty => Colors.Gray,
                FleetState.Ready => Colors.SpringGreen,
                FleetState.Sortie => Colors.Red,
                FleetState.Warning => Colors.Red,
                FleetState.Expedition => Colors.Aqua,
                FleetState.Fatigued => Colors.Orange,
                FleetState.Repairing => Colors.Orange,
                FleetState.Damaged => Colors.Orange,
                FleetState.Insufficient => Colors.Orange,
                _ => Colors.Transparent
            };
        }
    }
}

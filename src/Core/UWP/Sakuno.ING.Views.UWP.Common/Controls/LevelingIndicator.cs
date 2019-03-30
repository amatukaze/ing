using Sakuno.ING.Game.Models;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;

namespace Sakuno.ING.Views.UWP.Controls
{
    [TemplatePart(Name = nameof(PART_Range), Type = typeof(RangeBase))]
    [TemplatePart(Name = nameof(PART_Level), Type = typeof(TextBlock))]
    public sealed class LevelingIndicator : Control
    {
        public LevelingIndicator()
        {
            DefaultStyleKey = typeof(LevelingIndicator);
        }

        private Leveling _level;
        public Leveling Level
        {
            get => _level;
            set
            {
                _level = value;
                Update();
            }
        }

        public static DependencyProperty TextForegroundProperty =
            DependencyProperty.Register(nameof(TextForeground), typeof(Brush), typeof(LevelingIndicator),
                new PropertyMetadata(null));
        public Brush TextForeground
        {
            get => (Brush)GetValue(TextForegroundProperty);
            set => SetValue(TextForegroundProperty, value);
        }

        private RangeBase PART_Range;
        private TextBlock PART_Level;
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            PART_Range = GetTemplateChild(nameof(PART_Range)) as RangeBase;
            PART_Level = GetTemplateChild(nameof(PART_Level)) as TextBlock;
            Update();
        }

        private void Update()
        {
            if (PART_Range != null)
                if (_level.CurrentLevelExperience >= _level.NextLevelExperience)
                {
                    PART_Range.Minimum = 0;
                    PART_Range.Maximum = 1;
                    PART_Range.Value = 1;
                }
                else
                {
                    PART_Range.Minimum = _level.CurrentLevelExperience;
                    PART_Range.Maximum = _level.NextLevelExperience;
                    PART_Range.Value = _level.Experience;
                }

            if (PART_Level != null)
                PART_Level.Text = _level.Level.ToString();
        }
    }
}

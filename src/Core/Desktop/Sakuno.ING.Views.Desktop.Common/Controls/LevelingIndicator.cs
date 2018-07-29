using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using Sakuno.ING.Game.Models;

namespace Sakuno.ING.Views.Desktop.Controls
{
    [TemplatePart(Name = Name_PART_Range, Type = typeof(RangeBase))]
    [TemplatePart(Name = Name_PART_Level, Type = typeof(TextBlock))]
    public class LevelingIndicator : Control
    {
        private const string Name_PART_Range = "PART_Range";
        private const string Name_PART_Level = "PART_Level";

        static LevelingIndicator()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(LevelingIndicator), new FrameworkPropertyMetadata(typeof(LevelingIndicator)));
        }

        public static readonly DependencyProperty LevelProperty
            = DependencyProperty.Register(nameof(Level), typeof(Leveling), typeof(LevelingIndicator),
                new FrameworkPropertyMetadata(default(Leveling), (d, e) => ((LevelingIndicator)d).Update()));
        public Leveling Level
        {
            get => (Leveling)GetValue(LevelProperty);
            set => SetValue(LevelProperty, value);
        }

        public static readonly DependencyProperty TextForegroundProperty
            = DependencyProperty.Register(nameof(TextForeground), typeof(Brush), typeof(LevelingIndicator), new FrameworkPropertyMetadata(Brushes.Transparent, FrameworkPropertyMetadataOptions.AffectsRender));
        public Brush TextForeground
        {
            get => (Brush)GetValue(TextForegroundProperty);
            set => SetValue(TextForegroundProperty, value);
        }

        private RangeBase PART_Range;
        private TextBlock PART_Level;
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            PART_Range = GetTemplateChild(Name_PART_Range) as RangeBase;
            PART_Level = GetTemplateChild(Name_PART_Level) as TextBlock;
            Update();
        }

        private void Update()
        {
            var level = Level;

            if (PART_Range != null)
            {
                if (level.CurrentLevelExperience == level.NextLevelExperience)
                {
                    PART_Range.Minimum = 0;
                    PART_Range.Maximum = 1;
                    PART_Range.Value = 0;
                }
                else
                {
                    PART_Range.Minimum = level.CurrentLevelExperience;
                    PART_Range.Maximum = level.NextLevelExperience;
                    PART_Range.Value = level.Experience;
                }
            }

            if (PART_Level != null)
            {
                PART_Level.Text = level.Level.ToString();
            }
        }
    }
}

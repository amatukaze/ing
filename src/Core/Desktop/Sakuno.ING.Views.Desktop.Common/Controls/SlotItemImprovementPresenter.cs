using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace Sakuno.ING.Views.Desktop.Controls
{
    [TemplatePart(Name = Name_PART_Level, Type = typeof(Run))]
    public sealed class SlotItemImprovementPresenter : Control
    {
        private const string Name_PART_Level = "PART_Level";

        public static DependencyProperty LevelProperty =
            DependencyProperty.Register(nameof(Level), typeof(int), typeof(SlotItemImprovementPresenter), new PropertyMetadata(0, UpdateLevel));
        public int Level
        {
            get => (int)GetValue(LevelProperty);
            set => SetValue(LevelProperty, value);
        }

        public static DependencyProperty ShowStarProperty =
            DependencyProperty.Register(nameof(ShowStar), typeof(bool), typeof(SlotItemImprovementPresenter), new PropertyMetadata(true));
        public bool ShowStar
        {
            get => (bool)GetValue(ShowStarProperty);
            set => SetValue(ShowStarProperty, value);
        }

        private static string[] _strings = new string[10];

        private Run? _level;

        static SlotItemImprovementPresenter()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SlotItemImprovementPresenter), new FrameworkPropertyMetadata(typeof(SlotItemImprovementPresenter)));

            for (var i = 0; i < 9; i++)
                _strings[i] = "+" + (i + 1);

            _strings[9] = "max";
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _level = GetTemplateChild(Name_PART_Level) as Run;
        }

        private static void UpdateLevel(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var i = (SlotItemImprovementPresenter)d;
            var level = (int)e.NewValue;

            if (i._level is null)
                return;

            i._level.Text = level switch
            {
                0 => string.Empty,
                > 0 and <= 10 => _strings[level - 1],

                _ => throw new InvalidOperationException("Unsupported level: " + level),
            };
        }
    }
}

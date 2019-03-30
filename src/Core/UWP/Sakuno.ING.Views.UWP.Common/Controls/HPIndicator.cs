using Sakuno.ING.Game.Models;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;

namespace Sakuno.ING.Views.UWP.Controls
{
    [TemplatePart(Name = nameof(PART_Main), Type = typeof(RangeBase))]
    [TemplatePart(Name = nameof(PART_Addition), Type = typeof(RangeBase))]
    public class HPIndicator : Control
    {
        public HPIndicator()
        {
            DefaultStyleKey = typeof(HPIndicator);
        }

        private int _current, _max;
        public ShipHP HP
        {
            get => (_current, _max);
            set
            {
                if (_max != value.Max)
                {
                    _max = value.Max;
                    UpdateMax();
                }
                if (_current != value.Current)
                {
                    _current = value.Current;
                    UpdateCurrent();
                }
            }
        }

        private int? _additional;
        public int? Additional
        {
            get => _additional;
            set
            {
                if (_additional != value)
                {
                    _additional = value;
                    UpdateAdditional();
                }
            }
        }

        private Brush _additionalBrush;
        public Brush AdditionalBrush
        {
            get => _additionalBrush;
            set
            {
                _additionalBrush = value;
                if (PART_Addition != null)
                    PART_Addition.Foreground = value;
            }
        }

        private RangeBase PART_Main, PART_Addition;
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            PART_Main = GetTemplateChild(nameof(PART_Main)) as RangeBase;
            PART_Addition = GetTemplateChild(nameof(PART_Addition)) as RangeBase;
            if (PART_Main != null) PART_Main.Minimum = 0;
            if (PART_Addition != null)
            {
                PART_Addition.Minimum = 0;
                PART_Addition.Foreground = AdditionalBrush;
            }
            UpdateMax();
            UpdateCurrent();
            UpdateAdditional();
        }

        private void UpdateMax()
        {
            if (PART_Main != null) PART_Main.Maximum = _max;
            if (PART_Addition != null) PART_Addition.Maximum = _max;
        }

        private void UpdateCurrent()
        {
            if (PART_Main != null) PART_Main.Value = _current;
            VisualStateManager.GoToState(this, HP.DamageState.ToString(), true);
        }

        private void UpdateAdditional()
        {
            if (PART_Addition != null)
            {
                PART_Addition.Value = _additional ?? 0;
                PART_Addition.Visibility = _additional == null ? Visibility.Collapsed : Visibility.Visible;
            }
        }
    }
}

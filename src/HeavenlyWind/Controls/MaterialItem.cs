using Sakuno.KanColle.Amatsukaze.Game.Models;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Sakuno.KanColle.Amatsukaze.Controls
{
    class MaterialItem : Control
    {
        public static readonly DependencyProperty TypeProperty =
            DependencyProperty.Register(nameof(Type), typeof(MaterialType), typeof(MaterialItem));

        public MaterialType Type
        {
            get => (MaterialType)GetValue(TypeProperty);
            set => SetValue(TypeProperty, value);
        }

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(nameof(Value), typeof(int), typeof(MaterialItem), new PropertyMetadata(Int32Util.Zero, OnValueChanged, OnValueCoerce));

        static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) =>
            ((MaterialItem)d).OnValueChanged((int)e.NewValue);
        void OnValueChanged(int newValue) => IsRegenerating = newValue < RegenerationLimit;

        static object OnValueCoerce(DependencyObject d, object baseValue)
        {
            var value = (int)baseValue;

            if (value <= 0)
                return Int32Util.Zero;

            return baseValue;
        }

        public int Value
        {
            get => (int)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        public static readonly DependencyProperty PreviousDifferenceProperty =
            DependencyProperty.Register(nameof(PreviousDifference), typeof(int), typeof(MaterialItem), new PropertyMetadata(Int32Util.Zero, OnPreviousDifferenceChanged));

        static void OnPreviousDifferenceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) =>
            ((MaterialItem)d).OnPreviousDifferenceChanged((int)e.NewValue > 0);
        async void OnPreviousDifferenceChanged(bool isIncreased)
        {
            VisualStateManager.GoToState(this, "Updated", true);
            VisualStateManager.GoToState(this, isIncreased ? "Increment" : "Decrement", false);

            _updateAnimationNestingCount++;

            await Task.Delay(2000);

            _updateAnimationNestingCount--;

            if (_updateAnimationNestingCount > 0)
                return;

            VisualStateManager.GoToState(this, "Normal", true);
        }

        public int PreviousDifference
        {
            get => (int)GetValue(PreviousDifferenceProperty);
            set => SetValue(PreviousDifferenceProperty, value);
        }

        public static readonly DependencyProperty DayDifferenceProperty =
            DependencyProperty.Register(nameof(DayDifference), typeof(int), typeof(MaterialItem));

        public int DayDifference
        {
            get => (int)GetValue(DayDifferenceProperty);
            set => SetValue(DayDifferenceProperty, value);
        }

        public static readonly DependencyProperty WeekDifferenceProperty =
            DependencyProperty.Register(nameof(WeekDifference), typeof(int), typeof(MaterialItem));

        public int WeekDifference
        {
            get => (int)GetValue(WeekDifferenceProperty);
            set => SetValue(WeekDifferenceProperty, value);
        }

        public static readonly DependencyProperty MonthDifferenceProperty =
            DependencyProperty.Register(nameof(MonthDifference), typeof(int), typeof(MaterialItem));

        public int MonthDifference
        {
            get => (int)GetValue(MonthDifferenceProperty);
            set => SetValue(MonthDifferenceProperty, value);
        }

        public static readonly DependencyProperty RegenerationLimitProperty =
            DependencyProperty.Register(nameof(RegenerationLimit), typeof(int), typeof(MaterialItem));

        public int RegenerationLimit
        {
            get => (int)GetValue(RegenerationLimitProperty);
            set => SetValue(RegenerationLimitProperty, value);
        }

        static readonly DependencyPropertyKey IsRegeneratingPropertyKey =
            DependencyProperty.RegisterReadOnly(nameof(IsRegenerating), typeof(bool), typeof(MaterialItem), new PropertyMetadata(BooleanUtil.False));

        public static readonly DependencyProperty IsRegeneratingProperty = IsRegeneratingPropertyKey.DependencyProperty;

        public bool IsRegenerating
        {
            get => (bool)GetValue(IsRegeneratingProperty);
            private set => SetValue(IsRegeneratingPropertyKey, BooleanUtil.GetBoxed(value));
        }

        int _updateAnimationNestingCount;

        static MaterialItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MaterialItem), new FrameworkPropertyMetadata(typeof(MaterialItem)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            VisualStateManager.GoToState(this, "Normal", false);
            VisualStateManager.GoToState(this, "Increment", false);
        }
    }
}

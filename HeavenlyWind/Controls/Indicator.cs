using System;
using System.Windows;
using System.Windows.Controls;

namespace Sakuno.KanColle.Amatsukaze.Controls
{
    public class Indicator : Control
    {
        public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register(nameof(Maximum), typeof(int), typeof(Indicator),
                new FrameworkPropertyMetadata(100, (s, e) => ((Indicator)s).OnMaximumChanged((int)e.NewValue)));
        public int Maximum
        {
            get { return (int)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(nameof(Value), typeof(int), typeof(Indicator),
                new FrameworkPropertyMetadata(0, (s, e) => ((Indicator)s).OnValueChanged((int)e.NewValue), (s, rpValue) => ((int)rpValue).Clamp(0, ((Indicator)s).Maximum)));
        public int Value
        {
            get { return (int)GetValue(ValueProperty); }
            set { SetValue(MaximumProperty, value); }
        }

        public static readonly DependencyProperty MedianProperty = DependencyProperty.Register(nameof(Median), typeof(int), typeof(Indicator),
                new FrameworkPropertyMetadata(0, (s, e) => ((Indicator)s).OnMedianChanged((int)e.NewValue)));
        public int Median
        {
            get { return (int)GetValue(MedianProperty); }
            set { SetValue(MedianProperty, value); }
        }

        FrameworkElement r_Track;
        FrameworkElement r_Indicator;
        FrameworkElement r_MedianIndicator;

        static Indicator()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Indicator), new FrameworkPropertyMetadata(typeof(Indicator)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (r_Track != null)
                r_Track.SizeChanged -= Track_SizeChanged;

            r_Indicator = (FrameworkElement)Template.FindName("PART_Indicator", this);
            r_Track = (FrameworkElement)Template.FindName("PART_Track", this);
            r_MedianIndicator = (FrameworkElement)Template.FindName("PART_MedianIndicator", this);

            if (r_Track != null)
                r_Track.SizeChanged += Track_SizeChanged;
        }

        void Track_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            SetIndicatorLength(Value);
            SetMedianIndicatorLength(Median);
        }
        protected virtual void OnMaximumChanged(int rpValue) => SetIndicatorLength(Value);
        protected virtual void OnValueChanged(int rpValue) => SetIndicatorLength(rpValue);
        protected virtual void OnMedianChanged(int rpValue) => SetMedianIndicatorLength(rpValue);

        void SetMedianIndicatorLength(int rpValue)
        {
            if (r_MedianIndicator != null && Maximum != 0)
                r_MedianIndicator.Width = Math.Max(rpValue, 0) / (double)Maximum * r_Track.ActualWidth;
        }

        void SetIndicatorLength(int rpValue)
        {
            if (r_Track != null && r_Indicator != null && Maximum != 0)
                r_Indicator.Width = Math.Max(rpValue, 0) / (double)Maximum * r_Track.ActualWidth;
        }
    }
}

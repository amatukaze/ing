using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Sakuno.ING.Views.Desktop.Controls
{
    public class ProgressCircle : RangeBase
    {
        private readonly Ellipse circle = new Ellipse
        {
            RenderTransformOrigin = new Point(0.5, 0.5),
            RenderTransform = new RotateTransform(-90)
        };
        private readonly Ellipse background = new Ellipse();

        public ProgressCircle()
        {
            BindingOperations.SetBinding(background,
                Shape.StrokeProperty,
                new Binding(nameof(Background)) { Source = this });
            BindingOperations.SetBinding(circle,
                Shape.StrokeProperty,
                new Binding(nameof(Foreground)) { Source = this });

            BindingOperations.SetBinding(background,
                Shape.StrokeThicknessProperty,
                new Binding(nameof(StrokeThickness)) { Source = this });
            BindingOperations.SetBinding(circle,
                Shape.StrokeThicknessProperty,
                new Binding(nameof(StrokeThickness)) { Source = this });

            AddVisualChild(background);
            AddVisualChild(circle);
        }

        protected override int VisualChildrenCount => 2;
        protected override Visual GetVisualChild(int index)
        {
            switch (index)
            {
                case 0: return background;
                case 1: return circle;
                default: return null;
            }
        }

        protected override Size MeasureOverride(Size constraint)
        {
            if (double.IsPositiveInfinity(constraint.Width))
                if (double.IsPositiveInfinity(constraint.Height))
                    return default;
                else
                    return new Size(constraint.Height, constraint.Height);
            else if (double.IsPositiveInfinity(constraint.Height))
                return new Size(constraint.Width, constraint.Width);
            else
                return constraint;
        }

        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            double min = Math.Min(arrangeBounds.Width, arrangeBounds.Height);
            var rect = new Rect(0, 0, min, min);
            background.Arrange(rect);
            circle.Arrange(rect);
            diameter = min;
            UpdateBaseline();
            return new Size(min, min);
        }

        public static readonly DependencyProperty StrokeThicknessProperty
            = DependencyProperty.Register(nameof(StrokeThickness), typeof(double), typeof(ProgressCircle),
                new FrameworkPropertyMetadata(2.0, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure));
        public double StrokeThickness
        {
            get => (double)GetValue(StrokeThicknessProperty);
            set => SetValue(StrokeThicknessProperty, value);
        }

        private double diameter, roundRate;
        private void UpdateBaseline()
        {
            roundRate = (diameter - StrokeThickness) * Math.PI / StrokeThickness;
            circle.StrokeDashArray = new DoubleCollection { 0, roundRate * 2, roundRate * 2, 0 };
            UpdateOffset();
        }

        private void UpdateOffset()
        {
            double rate = (Value - Minimum) / (Maximum - Minimum);
            circle.StrokeDashOffset = -rate * roundRate;
        }

        protected override void OnMaximumChanged(double oldMaximum, double newMaximum) => UpdateOffset();
        protected override void OnMinimumChanged(double oldMinimum, double newMinimum) => UpdateOffset();
        protected override void OnValueChanged(double oldMinimum, double newMinimum) => UpdateOffset();
    }
}

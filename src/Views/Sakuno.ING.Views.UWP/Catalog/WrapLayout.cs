using System;
using Microsoft.UI.Xaml.Controls;
using Windows.Foundation;

namespace Sakuno.ING.Views.UWP.Catalog
{
    internal class WrapLayout : NonVirtualizingLayout
    {
        private double _horizontalSpacing;
        public double HorizontalSpacing
        {
            get => _horizontalSpacing;
            set
            {
                _horizontalSpacing = value;
                InvalidateArrange();
                InvalidateMeasure();
            }
        }

        private double _verticalSpacing;
        public double VerticalSpacing
        {
            get => _verticalSpacing;
            set
            {
                _verticalSpacing = value;
                InvalidateArrange();
                InvalidateMeasure();
            }
        }

        protected override Size MeasureOverride(NonVirtualizingLayoutContext context, Size availableSize)
        {
            Size total = default,
                parent = availableSize,
                line = default;

            foreach (var child in context.Children)
            {
                child.Measure(availableSize);
                var current = child.DesiredSize;
                if (current.Width == 0)
                    continue;

                double wChange = line.Width == 0 ?
                    current.Width :
                    current.Width + HorizontalSpacing;
                if (parent.Width >= wChange + line.Width)
                {
                    line.Width += wChange;
                    line.Height = Math.Max(line.Height, current.Height);
                }
                else
                {
                    total.Width = Math.Max(total.Width, line.Width);
                    total.Height += current.Height + VerticalSpacing;

                    if (parent.Width > current.Width)
                        line = current;
                    else
                    {
                        total.Width = Math.Max(total.Width, current.Width);
                        total.Height += current.Height;
                        line = default;
                    }
                }
            }

            return new Size(Math.Ceiling(Math.Max(line.Width, total.Width)), total.Height + line.Height);
        }

        protected override Size ArrangeOverride(NonVirtualizingLayoutContext context, Size finalSize)
        {
            var parent = finalSize;
            Point position = default;
            double currentHeight = 0;

            foreach (var child in context.Children)
            {
                var desired = child.DesiredSize;
                if (desired.Width == 0)
                    continue;

                if (desired.Width + position.X >= parent.Width)
                {
                    position.X = 0;
                    position.Y += currentHeight + HorizontalSpacing;
                    currentHeight = 0;
                }

                child.Arrange(new Rect(position, desired));
                position.X += desired.Width + HorizontalSpacing;
                currentHeight = Math.Max(desired.Height, currentHeight);
            }

            return finalSize;
        }
    }
}

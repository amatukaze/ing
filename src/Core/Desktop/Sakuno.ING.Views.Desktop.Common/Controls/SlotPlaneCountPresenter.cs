using System.Windows;
using System.Windows.Controls;

namespace Sakuno.ING.Views.Desktop.Controls
{
    public sealed class SlotPlaneCountPresenter : Control
    {
        public static readonly DependencyProperty PlaneIdProperty =
            DependencyProperty.Register(nameof(PlaneId), typeof(int), typeof(SlotPlaneCountPresenter),
                new PropertyMetadata(0));

        public int PlaneId
        {
            get => (int)GetValue(PlaneIdProperty);
            set => SetValue(PlaneIdProperty, value);
        }

        public static readonly DependencyProperty CurrentProperty =
            DependencyProperty.Register(nameof(Current), typeof(int), typeof(SlotPlaneCountPresenter),
                new PropertyMetadata(0));

        public int Current
        {
            get => (int)GetValue(CurrentProperty);
            set => SetValue(CurrentProperty, value);
        }

        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register(nameof(Maximum), typeof(int), typeof(SlotPlaneCountPresenter),
                new PropertyMetadata(0));

        public int Maximum
        {
            get => (int)GetValue(MaximumProperty);
            set => SetValue(MaximumProperty, value);
        }

        public static readonly DependencyProperty IsMaximumProperty =
            DependencyProperty.Register(nameof(IsMaximum), typeof(bool), typeof(SlotPlaneCountPresenter),
                new PropertyMetadata(true));

        public bool IsMaximum
        {
            get => (bool)GetValue(IsMaximumProperty);
            set => SetValue(IsMaximumProperty, value);
        }

        static SlotPlaneCountPresenter()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SlotPlaneCountPresenter), new FrameworkPropertyMetadata(typeof(SlotPlaneCountPresenter)));
        }
    }
}

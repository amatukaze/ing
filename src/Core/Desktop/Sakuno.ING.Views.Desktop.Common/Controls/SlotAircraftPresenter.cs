using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Sakuno.ING.Game.Models;

namespace Sakuno.ING.Views.Desktop.Controls
{
    public class SlotAircraftPresenter : Control
    {
        public static readonly DependencyProperty AircraftProperty
            = DependencyProperty.Register(nameof(Aircraft), typeof(ClampedValue), typeof(SlotAircraftPresenter),
                new PropertyMetadata(new ClampedValue(), (d, e) => ((SlotAircraftPresenter)d).UpdateCount()));
        public ClampedValue Aircraft
        {
            get => (ClampedValue)GetValue(AircraftProperty);
            set => SetValue(AircraftProperty, value);
        }

        public static readonly DependencyProperty IsPlaneProperty
            = DependencyProperty.Register(nameof(IsPlane), typeof(bool), typeof(SlotAircraftPresenter),
                new PropertyMetadata(false, (d, e) => ((SlotAircraftPresenter)d).CheckVisualState()));
        public bool IsPlane
        {
            get => (bool)GetValue(IsPlaneProperty);
            set => SetValue(IsPlaneProperty, value);
        }

        private void CheckVisualState()
        {
            var a = Aircraft;
            if (!IsPlane)
                textblock.Foreground = Brushes.Gray;
            else if (a.Current == a.Max)
                textblock.Foreground = Brushes.SpringGreen;
            else if (a.Current == 0)
                textblock.Foreground = Brushes.Red;
            else
                textblock.Foreground = Brushes.GreenYellow;
        }

        private void UpdateCount()
        {
            textblock.Text = Aircraft.Current.ToString();
            CheckVisualState();
        }

        private readonly TextBlock textblock = new TextBlock();

        public SlotAircraftPresenter() => AddVisualChild(textblock);
        protected override int VisualChildrenCount => 1;
        protected override Visual GetVisualChild(int index) => textblock;
    }
}

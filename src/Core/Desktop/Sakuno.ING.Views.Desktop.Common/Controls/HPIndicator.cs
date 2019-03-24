using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Sakuno.ING.Game.Models;

namespace Sakuno.ING.Views.Desktop.Controls
{
    [TemplatePart(Name = "PART_Track", Type = typeof(FrameworkElement))]
    [TemplatePart(Name = nameof(PART_Main), Type = typeof(FrameworkElement))]
    [TemplatePart(Name = nameof(PART_Addition), Type = typeof(FrameworkElement))]
    public class HPIndicator : Control
    {
        public static readonly DependencyProperty HPProperty
            = DependencyProperty.Register(nameof(HP), typeof(ShipHP), typeof(HPIndicator),
                new PropertyMetadata(new ShipHP(), (d, e) => ((HPIndicator)d).UpdateValue()));
        public ShipHP HP
        {
            get => (ShipHP)GetValue(HPProperty);
            set => SetValue(HPProperty, value);
        }

        public static readonly DependencyProperty AdditionalProperty
            = DependencyProperty.Register(nameof(Additional), typeof(int?), typeof(HPIndicator),
                new PropertyMetadata((d, e) => ((HPIndicator)d).UpdateAdditional()));
        public int? Additional
        {
            get => (int?)GetValue(AdditionalProperty);
            set => SetValue(AdditionalProperty, value);
        }

        public static readonly DependencyProperty AdditionalBrushProperty
            = DependencyProperty.Register(nameof(AdditionalBrush), typeof(Brush), typeof(HPIndicator),
                new PropertyMetadata());
        public Brush AdditionalBrush
        {
            get => (Brush)GetValue(AdditionalBrushProperty);
            set => SetValue(AdditionalBrushProperty, value);
        }

        static HPIndicator()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(HPIndicator), new FrameworkPropertyMetadata(typeof(HPIndicator)));
        }

        private FrameworkElement PART_Main, PART_Addition;
        private double? width;
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (GetTemplateChild("PART_Track") is FrameworkElement track)
            {
                track.SizeChanged += WidthChanged;
                width = track?.ActualWidth;
            }
            else
                width = null;
            PART_Main = GetTemplateChild(nameof(PART_Main)) as FrameworkElement;
            PART_Addition = GetTemplateChild(nameof(PART_Addition)) as FrameworkElement;
            UpdateValue();
        }

        private void WidthChanged(object sender, SizeChangedEventArgs e)
        {
            width = e.NewSize.Width;
            UpdateValue();
        }

        private void UpdateValue()
        {
            if (PART_Main != null && width is double w)
            {
                var hp = HP;
                PART_Main.Width = w * hp.Current / hp.Max;
                Foreground = hp.DamageState switch
                {
                    ShipDamageState.FullyHealthy => Brushes.SpringGreen,
                    ShipDamageState.Healthy => Brushes.Aquamarine,
                    ShipDamageState.LightlyDamaged => Brushes.YellowGreen,
                    ShipDamageState.ModeratelyDamaged => Brushes.Orange,
                    ShipDamageState.HeavilyDamaged => Brushes.Red,
                    ShipDamageState.Sunk => Brushes.Gray,
                    _ => null
                };
            }
            UpdateAdditional();
        }

        private void UpdateAdditional()
        {
            if (PART_Addition != null && width is double w && Additional is int a)
                PART_Addition.Width = w * a / HP.Max;
        }
    }
}

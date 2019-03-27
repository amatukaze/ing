using System.Windows;
using System.Windows.Controls;
using Sakuno.ING.Game.Models;
using Sakuno.ING.ViewModels.Homeport;

namespace Sakuno.ING.Views.Desktop.Controls
{
    public class LoSIndicator : Control
    {
        static LoSIndicator()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(LoSIndicator), new FrameworkPropertyMetadata(typeof(LoSIndicator)));
        }

        public FleetLoSVM ViewModel { get; } = new FleetLoSVM();

        public static readonly DependencyProperty EffectiveProperty
            = DependencyProperty.Register(nameof(Effective), typeof(LineOfSight), typeof(LoSIndicator),
                new PropertyMetadata(default(LineOfSight), (d, e) => ((LoSIndicator)d).ViewModel.Effective = (LineOfSight)e.NewValue));
        public LineOfSight Effective
        {
            get => (LineOfSight)GetValue(EffectiveProperty);
            set => SetValue(EffectiveProperty, value);
        }

        public static readonly DependencyProperty SimpleProperty
            = DependencyProperty.Register(nameof(Simple), typeof(int), typeof(LoSIndicator),
                new PropertyMetadata(default(int), (d, e) => ((LoSIndicator)d).ViewModel.Simple = (int)e.NewValue));
        public int Simple
        {
            get => (int)GetValue(SimpleProperty);
            set => SetValue(SimpleProperty, value);
        }
    }
}

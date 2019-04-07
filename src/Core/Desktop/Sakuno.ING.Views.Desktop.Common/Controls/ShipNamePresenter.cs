using System.Windows;
using System.Windows.Controls;
using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Views.Desktop.Controls
{
    public class ShipNamePresenter : Control
    {
        static ShipNamePresenter()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ShipNamePresenter), new FrameworkPropertyMetadata(typeof(ShipNamePresenter)));
        }

        public static readonly DependencyProperty SourceProperty
            = DependencyProperty.Register(nameof(Source), typeof(ShipName), typeof(ShipNamePresenter),
                new PropertyMetadata(null, (d, e) => ((ShipNamePresenter)d).Update()));

        public ShipName Source
        {
            get => (ShipName)GetValue(SourceProperty);
            set => SetValue(SourceProperty, value);
        }

        private static readonly object falseBox = false, trueBox = true;
        public static readonly DependencyProperty IsEliteProperty
            = DependencyProperty.Register(nameof(IsElite), typeof(bool), typeof(ShipNamePresenter), new PropertyMetadata(falseBox));
        public bool IsElite
        {
            get => (bool)GetValue(IsEliteProperty);
            private set => SetValue(IsEliteProperty, value);
        }

        public static readonly DependencyProperty IsFlagshipProperty
            = DependencyProperty.Register(nameof(IsFlagship), typeof(bool), typeof(ShipNamePresenter), new PropertyMetadata(falseBox));
        public bool IsFlagship
        {
            get => (bool)GetValue(IsFlagshipProperty);
            private set => SetValue(IsFlagshipProperty, value);
        }

        public static readonly DependencyProperty IsRemodelProperty
            = DependencyProperty.Register(nameof(IsRemodel), typeof(bool), typeof(ShipNamePresenter), new PropertyMetadata(falseBox));
        public bool IsRemodel
        {
            get => (bool)GetValue(IsRemodelProperty);
            private set => SetValue(IsRemodelProperty, value);
        }

        public static readonly DependencyProperty IsLateModelProperty
            = DependencyProperty.Register(nameof(IsLateModel), typeof(bool), typeof(ShipNamePresenter), new PropertyMetadata(falseBox));
        public bool IsLateModel
        {
            get => (bool)GetValue(IsLateModelProperty);
            private set => SetValue(IsLateModelProperty, value);
        }

        private void SetBoxedValue(DependencyProperty property, bool? value)
            => SetValue(property, value is true ? trueBox : falseBox);

        private void Update()
        {
            var @class = Source?.AbyssalClass;

            SetBoxedValue(IsRemodelProperty, @class?.HasFlag(AbyssalShipClass.Remodel));
            SetBoxedValue(IsLateModelProperty, @class?.HasFlag(AbyssalShipClass.LateModel));
            SetBoxedValue(IsEliteProperty, @class?.HasFlag(AbyssalShipClass.Elite));
            SetBoxedValue(IsFlagshipProperty, @class?.HasFlag(AbyssalShipClass.Flagship));
        }
    }
}

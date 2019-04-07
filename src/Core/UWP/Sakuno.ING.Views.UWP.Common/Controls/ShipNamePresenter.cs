using Sakuno.ING.Game.Models.MasterData;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Sakuno.ING.Views.UWP.Controls
{
    public class ShipNamePresenter : Control
    {
        public ShipNamePresenter()
        {
            DefaultStyleKey = typeof(ShipNamePresenter);
        }

        public static readonly DependencyProperty SourceProperty
            = DependencyProperty.Register(nameof(Source), typeof(ShipName), typeof(ShipNamePresenter),
                new PropertyMetadata(null, (d, e) => ((ShipNamePresenter)d).UpdateVisualState()));

        public ShipName Source
        {
            get => (ShipName)GetValue(SourceProperty);
            set => SetValue(SourceProperty, value);
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            UpdateVisualState();
        }

        private void UpdateVisualState()
        {
            var @class = Source?.AbyssalClass;

            if (@class?.HasFlag(AbyssalShipClass.Remodel) == true)
                VisualStateManager.GoToState(this, "Remodel", true);
            else
                VisualStateManager.GoToState(this, "NotRemodel", true);

            if (@class?.HasFlag(AbyssalShipClass.LateModel) == true)
                VisualStateManager.GoToState(this, "LateModel", true);
            else
                VisualStateManager.GoToState(this, "NotLateModel", true);

            if (@class?.HasFlag(AbyssalShipClass.Elite) == true)
                VisualStateManager.GoToState(this, "Elite", true);
            else
                VisualStateManager.GoToState(this, "NotElite", true);

            if (@class?.HasFlag(AbyssalShipClass.Flagship) == true)
                VisualStateManager.GoToState(this, "Flagship", true);
            else
                VisualStateManager.GoToState(this, "NotFlagship", true);
        }
    }
}

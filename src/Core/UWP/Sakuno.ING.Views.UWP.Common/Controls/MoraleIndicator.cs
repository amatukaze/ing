using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Sakuno.ING.Views.UWP.Controls
{
    public sealed class MoraleIndicator : Control
    {
        public MoraleIndicator()
        {
            DefaultStyleKey = typeof(MoraleIndicator);
        }

        public static readonly DependencyProperty MoraleProperty
            = DependencyProperty.Register(nameof(Morale), typeof(int), typeof(MoraleIndicator),
                new PropertyMetadata(0, (d, e) => ((MoraleIndicator)d).CheckVisualState()));
        public int Morale
        {
            get => (int)GetValue(MoraleProperty);
            set => SetValue(MoraleProperty, value);
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            CheckVisualState();
        }

        private void CheckVisualState()
        {
            int morale = Morale;
            if (morale > 85)
                VisualStateManager.GoToState(this, "Full", true);
            else if (morale > 49)
                VisualStateManager.GoToState(this, "High", true);
            else if (morale > 39)
                VisualStateManager.GoToState(this, "Normal", true);
            else if (morale > 29)
                VisualStateManager.GoToState(this, "Slight", true);
            else if (morale > 19)
                VisualStateManager.GoToState(this, "Moderate", true);
            else
                VisualStateManager.GoToState(this, "Serious", true);
        }
    }
}

using System.Windows.Media;

namespace Sakuno.KanColle.Amatsukaze.Controls
{
    public class ColorIndicator : Indicator
    {
        static readonly SolidColorBrush SightlyBrush;

        static ColorIndicator()
        {
            SightlyBrush = new SolidColorBrush(Color.FromRgb(240, 240, 0));
            SightlyBrush.Freeze();
        }

        protected override void OnValueChanged(int rpValue)
        {
            base.OnValueChanged(rpValue);
            SetIndicatorColor();
        }
        protected override void OnMaximumChanged(int rpValue)
        {
            base.OnMaximumChanged(rpValue);
            SetIndicatorColor();
        }

        void SetIndicatorColor()
        {
            var rRadio = Value / (double)Maximum;

            if (rRadio <= 0.25)
                Foreground = Brushes.Red;
            else if (rRadio <= 0.5)
                Foreground = Brushes.Orange;
            else if (rRadio <= 0.75)
                Foreground = SightlyBrush;
            else
                Foreground = Brushes.Green;
        }
    }
}

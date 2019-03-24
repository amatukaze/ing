using System.Windows;
using System.Windows.Controls;

namespace Sakuno.ING.Views.Desktop.Controls
{
    public class MoraleIndicator : Control
    {
        static MoraleIndicator()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MoraleIndicator), new FrameworkPropertyMetadata(typeof(MoraleIndicator)));
        }

        public static readonly DependencyProperty MoraleProperty
            = DependencyProperty.Register(nameof(Morale), typeof(int), typeof(MoraleIndicator),
                new PropertyMetadata(0, (d, e) => ((MoraleIndicator)d).CheckState()));
        public int Morale
        {
            get => (int)GetValue(MoraleProperty);
            set => SetValue(MoraleProperty, value);
        }

        public static readonly DependencyProperty StateProperty
            = DependencyProperty.Register(nameof(State), typeof(string), typeof(MoraleIndicator), new PropertyMetadata("Normal"));
        public string State
        {
            get => (string)GetValue(StateProperty);
            set => SetValue(StateProperty, value);
        }

        private void CheckState()
        {
            int v = Morale;
            if (v > 85)
                State = "Full";
            else if (v > 49)
                State = "High";
            else if (v > 39)
                State = "Normal";
            else if (v > 29)
                State = "Slight";
            else if (v > 19)
                State = "Moderate";
            else
                State = "Serious";
        }
    }
}

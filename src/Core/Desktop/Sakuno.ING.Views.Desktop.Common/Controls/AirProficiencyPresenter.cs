using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Sakuno.ING.Views.Desktop.Controls
{
    public class AirProficiencyPresenter : Control
    {
        public static DependencyProperty ProficiencyProperty
            = DependencyProperty.Register(nameof(Proficiency), typeof(int), typeof(AirProficiencyPresenter), new PropertyMetadata(0, Update));
        public int Proficiency
        {
            get => (int)GetValue(ProficiencyProperty);
            set => SetValue(ProficiencyProperty, value);
        }

        private readonly Image image = new Image();

        public AirProficiencyPresenter()
        {
            AddVisualChild(image);
        }

        protected override int VisualChildrenCount => 1;
        protected override Visual GetVisualChild(int index) => image;

        private static readonly Dictionary<int, BitmapImage> bitmapSources = new Dictionary<int, BitmapImage>();
        private static void Update(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var i = (AirProficiencyPresenter)d;
            var level = (int)e.NewValue;

            if (level == 0)
            {
                i.image.Source = null;
            }

            if (!bitmapSources.TryGetValue(level, out var source))
            {
                try
                {
                    source = new BitmapImage(new Uri($"pack://application:,,,/Sakuno.ING.Views.Desktop.Common;component/Images/AirProficiency/{level}.png"));
                    source.Freeze();
                }
                catch
                {
                    source = null;
                }
                bitmapSources[level] = source;
            }

            i.image.Source = source;
        }
    }
}

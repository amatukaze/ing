using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Sakuno.ING.Views.Desktop.Controls
{
    public class EquipmentIcon : Control
    {
        public static readonly DependencyProperty IdProperty
            = DependencyProperty.Register(nameof(Id), typeof(int), typeof(UseItemIcon), new PropertyMetadata(0, Update));

        public int Id
        {
            get => (int)GetValue(IdProperty);
            set => SetValue(IdProperty, value);
        }

        private readonly Image image = new Image();

        protected override int VisualChildrenCount => 1;
        protected override Visual GetVisualChild(int index) => image;

        private static readonly Dictionary<int, BitmapImage> bitmapSources = new Dictionary<int, BitmapImage>();
        private static void Update(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var i = (EquipmentIcon)d;
            var id = (int)e.NewValue;

            if (!bitmapSources.TryGetValue(id, out var source))
            {
                try
                {
                    source = new BitmapImage(new Uri($"pack://application:,,,/Sakuno.ING.Views.Desktop.Common;component/Images/Equipment/{id}.png"));
                    source.Freeze();
                }
                catch
                {
                    source = null;
                }
                bitmapSources[id] = source;
            }

            i.image.Source = source;
        }
    }
}

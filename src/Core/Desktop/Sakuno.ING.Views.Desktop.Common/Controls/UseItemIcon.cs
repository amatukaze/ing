using Sakuno.ING.Game.Models.Knowledge;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Sakuno.ING.Views.Desktop.Controls
{
    public class UseItemIcon : Control
    {
        public static readonly DependencyProperty IdProperty =
            DependencyProperty.Register(nameof(Id), typeof(KnownUseItem), typeof(UseItemIcon), new PropertyMetadata(default(KnownUseItem), Update));

        public KnownUseItem Id
        {
            get => (KnownUseItem)GetValue(IdProperty);
            set => SetValue(IdProperty, value);
        }

        private readonly Image image = new Image();

        public UseItemIcon()
        {
            AddVisualChild(image);
        }

        protected override int VisualChildrenCount => 1;
        protected override Visual GetVisualChild(int index) => image;

        private static readonly SortedList<KnownUseItem, BitmapImage?> bitmapSources = new SortedList<KnownUseItem, BitmapImage?>();
        private static void Update(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var i = (UseItemIcon)d;
            var id = (KnownUseItem)e.NewValue;

            if (!bitmapSources.TryGetValue(id, out var source))
            {
                try
                {
                    source = new BitmapImage(new Uri($"pack://application:,,,/Sakuno.ING.Views.Desktop.Common;component/Images/UseItem/{(int)id}.png"));
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

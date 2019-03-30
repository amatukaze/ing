using System;
using Sakuno.ING.Game.Models.Knowledge;
using Windows.ApplicationModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace Sakuno.ING.Views.UWP.Controls
{
    public sealed class UseItemIcon : UserControl
    {
        private readonly Image _image = new Image();
        public UseItemIcon() => Content = _image;

        public static readonly DependencyProperty IdProperty
            = DependencyProperty.Register(nameof(Id), typeof(KnownUseItem), typeof(UseItemIcon),
                new PropertyMetadata((KnownUseItem)0, (d, e) => ((UseItemIcon)d).Update()));
        public KnownUseItem Id
        {
            get => (KnownUseItem)GetValue(IdProperty);
            set => SetValue(IdProperty, value);
        }

        private void Update()
        {
            int intId = (int)Id;
            if (DesignMode.DesignModeEnabled)
                _image.Source = new BitmapImage(new Uri($"ms-appx:///Sakuno.ING.Views.UWP.Common/Assets/Images/UseItem/{intId}.png"));
            else
            {
                string stringId = "UseItemIcon_Bitmap_" + intId;
                if (Application.Current.Resources.TryGetValue(stringId, out object source))
                    _image.Source = source as BitmapImage;
                else
                {
                    var s = new BitmapImage(new Uri($"ms-appx:///Sakuno.ING.Views.UWP.Common/Assets/Images/UseItem/{intId}.png"));
                    Application.Current.Resources[stringId] = s;
                    _image.Source = s;
                }
            }
        }
    }
}

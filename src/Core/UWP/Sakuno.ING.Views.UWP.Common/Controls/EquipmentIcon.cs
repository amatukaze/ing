using System;
using Windows.ApplicationModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace Sakuno.ING.Views.UWP.Controls
{
    public sealed class EquipmentIcon : UserControl
    {
        private readonly Image _image = new Image();
        public EquipmentIcon() => Content = _image;

        private int _id;
        public int Id
        {
            get => _id;
            set
            {
                if (_id != value)
                {
                    _id = value;
                    if (value == 0)
                        _image.Source = null;
                    else if (DesignMode.DesignModeEnabled)
                        _image.Source = new BitmapImage(new Uri($"ms-appx:///Sakuno.ING.Views.UWP.Common/Assets/Images/Equipment/{value}.png"));
                    else
                    {
                        string stringId = "EquipmentIcon_Bitmap_" + value;
                        if (Application.Current.Resources.TryGetValue(stringId, out object source))
                            _image.Source = source as BitmapImage;
                        else
                        {
                            var s = new BitmapImage(new Uri($"ms-appx:///Sakuno.ING.Views.UWP.Common/Assets/Images/Equipment/{value}.png"));
                            Application.Current.Resources[stringId] = s;
                            _image.Source = s;
                        }
                    }
                }
            }
        }
    }
}

using System;
using System.Linq;
using Microsoft.Graphics.Canvas.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace Sakuno.KanColle.Amatsukaze.UWP.Views.Settings
{
    internal class FontSelection
    {
        public FontSelection(string display, string name, FontFamily fontFamily)
        {
            Display = display;
            Name = name;
            FontFamily = fontFamily;
        }

        public string Display { get; }
        public string Name { get; }
        public FontFamily FontFamily { get; }
    };
    public sealed partial class FontPicker : UserControl
    {
        private const string @default = "(default)";

        public FontPicker()
        {
            this.InitializeComponent();
            PreviewText.Text = @default;
            FontFamily = FontFamily.XamlAutoFontFamily;
        }

        public string SelectedFont
        {
            get => (string)GetValue(SelectedFontProperty);
            set => SetValue(SelectedFontProperty, value);
        }

        public static readonly DependencyProperty SelectedFontProperty =
            DependencyProperty.Register(nameof(SelectedFont), typeof(string), typeof(FontPicker), new PropertyMetadata(string.Empty, FontChanged));

        private static void FontChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var p = (FontPicker)d;
            var fontName = (string)e.NewValue;
            if (string.IsNullOrEmpty(fontName))
            {
                p.PreviewText.Text = @default;
                p.FontFamily = FontFamily.XamlAutoFontFamily;
            }
            else
            {
                p.PreviewText.Text = fontName;
                p.FontFamily = new FontFamily(fontName);
            }
        }

        private async void Button_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var fonts = CanvasTextFormat.GetSystemFontFamilies();
            var dialog = new FontPickerDialog(fonts
                .Select(f => new FontSelection(f, f, new FontFamily(f)))
                .Prepend(new FontSelection(@default, string.Empty, FontFamily.XamlAutoFontFamily)));
            if (await dialog.ShowAsync() == ContentDialogResult.Primary)
                SelectedFont = dialog.SelectedFont.Name;
        }
    }
}

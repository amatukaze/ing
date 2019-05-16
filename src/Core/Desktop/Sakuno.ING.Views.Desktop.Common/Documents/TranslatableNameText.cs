using System.Windows;
using Sakuno.ING.Game;

namespace Sakuno.ING.Views.Desktop.Documents
{
    public class TranslatableNameText : TranslatableText
    {
        public static readonly DependencyProperty SourceProperty
            = DependencyProperty.Register(nameof(Source), typeof(TextTranslationDescriptor), typeof(TranslatableNameText),
                new PropertyMetadata((d, e) =>
                {
                    var c = (TranslatableText)d;
                    var value = (TextTranslationDescriptor)e.NewValue;
                    if (value is null)
                        c.Text = string.Empty;
                    else if (Translate)
                        c.Text = Localization.GetLocalized(value.Category, value.Id.ToString()) ?? value.Origin ?? string.Empty;
                    else if (value.PreferOrigin)
                        c.Text = value.Origin ?? string.Empty;
                    else
                        c.Text = Localization.GetUnlocalized(value.Category, value.Id.ToString()) ?? value.Origin ?? string.Empty;
                }));

        public TextTranslationDescriptor Source
        {
            get => (TextTranslationDescriptor)GetValue(SourceProperty);
            set => SetValue(SourceProperty, value);
        }
    }
}

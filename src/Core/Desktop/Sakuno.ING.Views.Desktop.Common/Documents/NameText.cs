using System.Windows;
using Sakuno.ING.Game;

namespace Sakuno.ING.Views.Desktop.Documents
{
    public class NameText : TranslatableText
    {
        public static readonly DependencyProperty SourceProperty
            = DependencyProperty.Register(nameof(Source), typeof(TextTranslationGroup), typeof(NameText),
                new PropertyMetadata((d, e) =>
                {
                    var c = (NameText)d;
                    var value = e.NewValue as TextTranslationGroup;
                    if (Translate)
                        c.Text = value?.Translation;
                    else
                        c.Text = value?.Origin;
                }));

        public TextTranslationGroup Source
        {
            get => (TextTranslationGroup)GetValue(SourceProperty);
            set => SetValue(SourceProperty, value);
        }
    }
}

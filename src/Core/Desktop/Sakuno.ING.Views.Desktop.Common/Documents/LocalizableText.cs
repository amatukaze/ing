using System;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Markup;
using Sakuno.ING.Game;

namespace Sakuno.ING.Views.Desktop.Documents
{
    public class LocalizableText : Run, IServiceProvider, IProvideValueTarget
    {
        public static readonly object ShouldTranslateKey = new object();

        public static readonly DependencyProperty SourceProperty
            = DependencyProperty.Register(nameof(Source), typeof(TextTranslationGroup), typeof(LocalizableText), new PropertyMetadata(Update));

        public TextTranslationGroup Source
        {
            get => (TextTranslationGroup)GetValue(SourceProperty);
            set => SetValue(SourceProperty, value);
        }

        private static readonly DependencyProperty ShouldTranslateProperty
            = DependencyProperty.Register("ShoudTranslate", typeof(bool), typeof(LocalizableText), new PropertyMetadata(false, Update));

        public LocalizableText()
        {
            SetValue(ShouldTranslateProperty, new DynamicResourceExtension(ShouldTranslateKey).ProvideValue(this));
            Update();
        }

        private static void Update(DependencyObject d, DependencyPropertyChangedEventArgs e)
            => ((LocalizableText)d).Update();

        private void Update()
        {
            if (GetValue(ShouldTranslateProperty) is true)
                Text = Source?.Translation;
            else
                Text = Source?.Origin;
        }

        object IServiceProvider.GetService(Type serviceType)
            => serviceType == typeof(IProvideValueTarget) ? this : null;
        object IProvideValueTarget.TargetObject => this;
        object IProvideValueTarget.TargetProperty => ShouldTranslateProperty;
    }
}

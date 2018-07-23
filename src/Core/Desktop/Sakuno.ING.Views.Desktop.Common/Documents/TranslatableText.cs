using System;
using System.Windows.Documents;
using System.Windows.Markup;
using Sakuno.ING.Composition;
using Sakuno.ING.Settings;

namespace Sakuno.ING.Views.Desktop.Documents
{
    public abstract class TranslatableText : Run
    {
        private static readonly Lazy<bool> translateLazy
            = new Lazy<bool>(() => Compositor.Static<LocaleSetting>().TranslateContent.InitialValue);

        protected static bool Translate => translateLazy.Value;

        private static readonly XmlLanguage japanese = XmlLanguage.GetLanguage("ja-jp");

        protected TranslatableText()
        {
            if (!Translate)
                Language = japanese;
        }
    }
}

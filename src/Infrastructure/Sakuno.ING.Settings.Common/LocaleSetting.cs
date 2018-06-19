using Sakuno.ING.Composition;

namespace Sakuno.ING.Settings
{
    [Export(typeof(LocaleSetting))]
    public class LocaleSetting
    {
        public LocaleSetting(ISettingsManager manager)
        {
            Language = manager.Register("application_language", string.Empty);
            UserLanguageFont = manager.Register("application_font_user", string.Empty);
            ContentLanguageFont = manager.Register("application_font_content", string.Empty);
            TranslateContent = manager.Register("application_translate", false);
        }

        public ISettingItem<string> Language { get; }
        public ISettingItem<string> UserLanguageFont { get; }
        public ISettingItem<string> ContentLanguageFont { get; }
        public ISettingItem<bool> TranslateContent { get; }
    }
}

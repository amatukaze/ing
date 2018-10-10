using Sakuno.ING.Composition;

namespace Sakuno.ING.Settings
{
    [Export(typeof(LocaleSetting))]
    public class LocaleSetting
    {
        public LocaleSetting(ISettingsManager manager)
        {
            Language = manager.Register("application.language", string.Empty);
            UserLanguageFont = manager.Register("application.font.user", string.Empty);
            ContentLanguageFont = manager.Register("application.font.content", string.Empty);
            TranslateContent = manager.Register("application.translate_content", false);
        }

        public ISettingItem<string> Language { get; }
        public ISettingItem<string> UserLanguageFont { get; }
        public ISettingItem<string> ContentLanguageFont { get; }
        public ISettingItem<bool> TranslateContent { get; }
    }
}

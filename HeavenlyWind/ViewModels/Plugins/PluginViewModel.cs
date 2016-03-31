using Sakuno.KanColle.Amatsukaze.Services.Plugins;

namespace Sakuno.KanColle.Amatsukaze.ViewModels.Plugins
{
    public class PluginViewModel : ModelBase
    {
        Plugin r_Plugin;

        public string Name => r_Plugin.Name;
        public string Author => r_Plugin.Author;
        public string Version => r_Plugin.Version;

        internal PluginViewModel(Plugin rpPlugin)
        {
            r_Plugin = rpPlugin;
        }
    }
}

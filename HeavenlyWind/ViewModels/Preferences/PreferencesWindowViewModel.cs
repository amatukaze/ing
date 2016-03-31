using Sakuno.KanColle.Amatsukaze.Services;
using Sakuno.KanColle.Amatsukaze.ViewModels.Plugins;
using System.Collections.Generic;
using System.Linq;

namespace Sakuno.KanColle.Amatsukaze.ViewModels.Preferences
{
    public class PreferencesWindowViewModel : WindowViewModel
    {
        public static PreferencesWindowViewModel Instance { get; } = new PreferencesWindowViewModel();

        public IList<PluginViewModel> LoadedPlugins { get; }

        PreferencesWindowViewModel()
        {
            LoadedPlugins = PluginService.Instance.LoadedPlugins.Select(r => new PluginViewModel(r)).ToList().AsReadOnly();
        }
    }
}

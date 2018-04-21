using Sakuno.KanColle.Amatsukaze.Composition;
using Sakuno.KanColle.Amatsukaze.Settings;
using Sakuno.KanColle.Amatsukaze.Shell;

namespace Sakuno.KanColle.Amatsukaze.UWP.Views.Settings
{
    internal class Module : IModule
    {
        public void Initialize(IResolver resolver)
        {
            var shell = resolver.Resolve<IShell>();
            shell.RegisterSettingView(typeof(LocaleSettingView), SettingCategory.Appearance);
        }
    }
}

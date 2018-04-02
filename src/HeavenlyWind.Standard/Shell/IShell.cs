using System;
using Sakuno.KanColle.Amatsukaze.Settings;

namespace Sakuno.KanColle.Amatsukaze.Shell
{
    public interface IShell
    {
        void Run();
        void RegisterView(Type viewType, string id, bool isFixedSize = true, bool singleWindowRecommended = false);
        void RegisterSettingView(Type viewType, SettingCategory category = SettingCategory.Misc);
    }
}

using System;
using Sakuno.ING.Settings;

namespace Sakuno.ING.Shell
{
    public interface IShell
    {
        void Run();
        void RegisterView(Type viewType, string id, bool isFixedSize = true, bool singleWindowRecommended = false);
        void RegisterSettingView(Type viewType, SettingCategory category = SettingCategory.Misc);
    }
}

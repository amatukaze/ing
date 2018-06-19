using System;
using Sakuno.ING.Settings;

namespace Sakuno.ING.Composition
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class ExportSettingViewAttribute : Attribute
    {
        public SettingCategory Category { get; } = SettingCategory.Misc;

        public ExportSettingViewAttribute(SettingCategory category = SettingCategory.Misc) => Category = category;
    }
}

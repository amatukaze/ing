using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Sakuno.ING.Composition;
using Sakuno.ING.Localization;
using Sakuno.ING.Settings;
using Windows.UI.Xaml.Controls;

namespace Sakuno.ING.UWP
{
    internal class SettingsPageModel
    {
        public string Title;
        public object[] Content;
    }

    public sealed partial class SettingsView : Page
    {
        private readonly SettingsPageModel[] Pages;
        private readonly ILocalizationService localize;
        public SettingsView(List<(Type ViewType, SettingCategory Category)> entries)
        {
            this.InitializeComponent();
            localize = StaticResolver.Instance.Resolve<ILocalizationService>();
            Pages = entries
                .GroupBy(e => e.Category)
                .OrderBy(e => e.Key)
                .Select(e => new SettingsPageModel
                {
                    Title = localize.GetLocalized("SettingCategory", e.Key.ToString()) ?? e.Key.ToString(),
                    Content = e.Select(t => Activator.CreateInstance(t.ViewType)).ToArray()
                }).ToArray();
        }
    }
}

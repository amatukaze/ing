using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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
        public SettingsView(List<(Type ViewType, SettingCategory Category)> entries, ILocalizationService localization)
        {
            this.InitializeComponent();
            Pages = entries
                .GroupBy(e => e.Category)
                .OrderBy(e => e.Key)
                .Select(e => new SettingsPageModel
                {
                    Title = localization.GetLocalized("SettingCategory", e.Key.ToString()) ?? e.Key.ToString(),
                    Content = e.Select(t => Activator.CreateInstance(t.ViewType)).ToArray()
                }).ToArray();
        }
    }
}

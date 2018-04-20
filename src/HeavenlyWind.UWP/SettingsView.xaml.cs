using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Sakuno.KanColle.Amatsukaze.Composition;
using Sakuno.KanColle.Amatsukaze.Settings;
using Sakuno.KanColle.Amatsukaze.ViewModels;
using Windows.UI.Xaml.Controls;

namespace Sakuno.KanColle.Amatsukaze.UWP
{
    internal class SettingsPageModel
    {
        public LocalizableText Title;
        public object[] Content;
    }

    public sealed partial class SettingsView : Page
    {
        private readonly SettingsPageModel[] Pages;
        private readonly LocalizableTextStore textStore;
        public SettingsView(List<(Type ViewType, SettingCategory Category)> entries)
        {
            this.InitializeComponent();
            textStore = StaticResolver.Instance.Resolve<LocalizableTextStore>();
            Pages = entries
                .GroupBy(e => e.Category)
                .OrderBy(e => e.Key)
                .Select(e => new SettingsPageModel
                {
                    Title = textStore.GetText("SettingCategory", e.Key.ToString(), e.Key.ToString()),
                    Content = e.Select(t => Activator.CreateInstance(t.ViewType)).ToArray()
                }).ToArray();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Sakuno.KanColle.Amatsukaze.Settings;
using Windows.UI.Xaml.Controls;

//https://go.microsoft.com/fwlink/?LinkId=234236 上介绍了“用户控件”项模板

namespace Sakuno.KanColle.Amatsukaze.UWP
{

    internal class SettingsPageModel
    {
        public string Title; //TODO:localize
        public object[] Content;
    }

    public sealed partial class SettingsView : UserControl
    {
        private SettingsPageModel[] Pages;
        public SettingsView(List<(Type ViewType, SettingCategory Category)> entries)
        {
            this.InitializeComponent();
            Pages = entries
                .GroupBy(e => e.Category)
                .OrderBy(e => e.Key)
                .Select(e => new SettingsPageModel
                {
                    Title = e.Key.ToString(),
                    Content = e.Select(t => Activator.CreateInstance(t.ViewType)).ToArray()
                }).ToArray();
        }
    }
}

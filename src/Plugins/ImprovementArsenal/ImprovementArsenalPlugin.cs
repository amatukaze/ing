using Sakuno.KanColle.Amatsukaze.Extensibility;
using Sakuno.KanColle.Amatsukaze.Plugin.ImprovementArsenal.Internal;
using Sakuno.KanColle.Amatsukaze.Plugin.ImprovementArsenal.ViewModels;
using Sakuno.KanColle.Amatsukaze.Plugin.ImprovementArsenal.Views;
using System;
using System.ComponentModel.Composition;
using System.Windows;

namespace Sakuno.KanColle.Amatsukaze.Plugin.ImprovementArsenal
{
    [PluginExport("ImprovementArsenal", "Kodama Sakuno", "v1β6", "8DA683BD-B3F9-4207-B336-E1373968D6E4")]
    [Export(typeof(IToolPane))]
    class ImprovementArsenalPlugin : ModelBase, IPlugin, IToolPane
    {
        public string Name => InternalStringResources.Instance.Strings["Name"];

        public Lazy<object> View { get; } = new Lazy<object>(() => new ImprovementArsenalToolPane() { DataContext = new MainViewModel() });

        public ImprovementArsenalPlugin()
        {
            PropertyChangedEventListener.FromSource(InternalStringResources.Instance).Add(nameof(InternalStringResources.Instance.Strings), (s, e) => OnPropertyChanged(nameof(Name)));
        }

        public void Initialize()
        {
        }
    }
}

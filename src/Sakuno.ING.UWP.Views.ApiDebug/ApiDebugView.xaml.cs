using System;
using System.Collections.ObjectModel;
using Newtonsoft.Json.Linq;
using Sakuno.ING.Composition;
using Sakuno.ING.Game;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;

namespace Sakuno.ING.UWP.Views.ApiDebug
{
    internal class Message
    {
        public string Name;
        public DateTimeOffset TimeStamp;
        public JToken Body;
    }
    public sealed partial class ApiDebugView : UserControl
    {
        private readonly GameListener Provider = StaticResolver.Instance.Resolve<GameListener>();
        private readonly ObservableCollection<Message> Sessions = new ObservableCollection<Message>();
        public ApiDebugView()
        {
            this.InitializeComponent();
            Provider.RegisterAny().Received +=
                async obj => await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    Sessions.Add(new Message
                    {
                        Name = obj.Name,
                        TimeStamp = obj.TimeStamp,
                        Body = obj.Response
                    });
                    while (Sessions.Count >= 50)
                        Sessions.RemoveAt(0);
                });
        }
    }
}

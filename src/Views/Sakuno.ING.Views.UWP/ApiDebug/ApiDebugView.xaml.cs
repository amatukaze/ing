using System;
using System.Collections.ObjectModel;
using Newtonsoft.Json.Linq;
using Sakuno.ING.Game;
using Sakuno.ING.Shell;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;

namespace Sakuno.ING.Views.UWP.ApiDebug
{
    internal class Message
    {
        public string Name;
        public DateTimeOffset TimeStamp;
        public JToken Body;
    }
    [ExportView("ApiDebug")]
    public sealed partial class ApiDebugView : UserControl
    {
        private readonly ObservableCollection<Message> Sessions = new ObservableCollection<Message>();
        public ApiDebugView(GameProvider provider)
        {
            this.InitializeComponent();
            provider.RegisterAny().Received +=
                async (timeStamp, obj) => await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    Sessions.Add(new Message
                    {
                        Name = obj.Name,
                        TimeStamp = timeStamp,
                        Body = obj.Response
                    });
                    while (Sessions.Count >= 50)
                        Sessions.RemoveAt(0);
                });
        }
    }
}

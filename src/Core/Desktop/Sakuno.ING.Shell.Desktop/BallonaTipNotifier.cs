using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Sakuno.ING.Composition;
using Sakuno.ING.Notification;
using Sakuno.ING.Timing;

namespace Sakuno.ING.Shell.Desktop
{
    [Export(typeof(INotifier))]
    internal sealed class BallonTipNotifier : INotifier
    {
        public bool IsSupported => true;
        public string Id => "BallonTip";

        private readonly ITimingService timing;
        private readonly Dictionary<string, (DateTimeOffset time, string title, string content)> map = new Dictionary<string, (DateTimeOffset, string, string)>();

        public BallonTipNotifier(ITimingService timing)
        {
            this.timing = timing;
        }

        private NotifyIcon icon;
        public void Initialize()
        {
            icon = new NotifyIcon
            {
                Icon = Icon.ExtractAssociatedIcon(Process.GetCurrentProcess().MainModule.FileName),
                Text = "Intelligent Naval Gun",
                Visible = true
            };

            timing.Elapsed += Elapsed;
        }

        public void Deinitialize()
        {
            icon.Dispose();
            icon = null;
            timing.Elapsed -= Elapsed;
        }

        private void Elapsed(DateTimeOffset t)
        {
            List<string> removed = null;
            lock (map)
            {
                foreach (var kvp in map)
                    if (kvp.Value.time <= t)
                    {
                        if (removed is null) removed = new List<string>();
                        removed.Add(kvp.Key);
                        icon.ShowBalloonTip(0, kvp.Value.title, kvp.Value.content, ToolTipIcon.None);
                    }
                if (removed != null)
                    foreach (var r in removed)
                        map.Remove(r);
            }
        }

        public void AddSchedule(string id, string title, string content, DateTimeOffset time)
        {
            lock (map)
                map[id] = (time, title, content);
        }

        public void RemoveSchedule(string id)
        {
            lock (map)
                map.Remove(id);
        }
    }
}

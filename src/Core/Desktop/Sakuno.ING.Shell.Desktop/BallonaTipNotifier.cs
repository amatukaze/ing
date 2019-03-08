using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Sakuno.ING.Composition;
using Sakuno.ING.Notification;

namespace Sakuno.ING.Shell.Desktop
{
    [Export(typeof(INotifier))]
    internal sealed class BallonTipNotifier : INotifier
    {
        public bool IsSupported => true;
        public string Id => "BallonTip";

        private readonly IShell shell;
        public BallonTipNotifier(IShell shell) => this.shell = shell;

        private NotifyIcon icon;
        public void Initialize()
        {
            icon = new NotifyIcon
            {
                Icon = Icon.ExtractAssociatedIcon(Process.GetCurrentProcess().MainModule.FileName),
                Text = "Intelligent Naval Gun",
                Visible = true
            };

            shell.Exited += () => icon.Dispose();
        }

        public void Show(string title, string content, string sound)
        {
            icon.ShowBalloonTip(0, title, content, ToolTipIcon.None);
        }
    }
}

using ReactiveUI;
using Sakuno.ING.Composition;
using Splat;

namespace Sakuno.ING.Shell.Desktop
{
    [Export(typeof(IShell))]
    internal class Shell : IShell
    {
        public void Run()
        {
            var app = new ShellApp();

            Locator.CurrentMutable.InitializeReactiveUI();

            app.Run();
        }
    }
}

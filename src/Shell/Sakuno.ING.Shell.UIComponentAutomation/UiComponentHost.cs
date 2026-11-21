using Avalonia.Headless;
using Avalonia.Threading;

namespace Sakuno.ING.Shell.UIComponentAutomation;

internal static class UiComponentHost
{
    private static readonly Lazy<HeadlessUnitTestSession> Session = new(() =>
        HeadlessUnitTestSession.StartNew(typeof(AutomationApplication), AvaloniaTestIsolationLevel.PerAssembly));

    public static T Dispatch<T>(Func<T> action) =>
        Session.Value.Dispatch(action, CancellationToken.None).GetAwaiter().GetResult();

    public static void Settle(Window window)
    {
        Dispatcher.UIThread.RunJobs(DispatcherPriority.Loaded);
        window.UpdateLayout();
        Dispatcher.UIThread.RunJobs(DispatcherPriority.Render);
        Dispatcher.UIThread.RunJobs(DispatcherPriority.Background);
    }
}

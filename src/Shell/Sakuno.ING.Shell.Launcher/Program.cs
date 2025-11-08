using System;
using Avalonia;

namespace Sakuno.ING.Shell.Launcher;

internal sealed class Program
{
    [STAThread]
    public static void Main(string[] args) =>
        BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);

    public static AppBuilder BuildAvaloniaApp() =>
        AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .UseShell()
            .WithInterFont()
            .LogToTrace();
}

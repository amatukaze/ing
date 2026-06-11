using System;
using System.Text;
using Avalonia;

namespace Sakuno.ING.Shell.Launcher;

internal sealed class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;

        BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);
    }

    public static AppBuilder BuildAvaloniaApp() =>
        AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .UseShell()
            .WithInterFont()
            .LogToTrace();
}

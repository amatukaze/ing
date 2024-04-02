using System.Windows;

namespace Sakuno.ING.Shell.Hosting;

public class ShellBuilder
{
    internal Type? ApplicationType { get; set; }
    internal Type? MainWindowType { get; set; }

    public void UseApplication<TApplication>() where TApplication : Application
    {
        ApplicationType = typeof(TApplication);
    }

    public void UseMainWindow<TWindow>() where TWindow : Window, IShellMainWindow
    {
        MainWindowType = typeof(TWindow);
    }
}

using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using DryIoc;
using DryIoc.Microsoft.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Sakuno.ING.ViewModels;

namespace Sakuno.ING.Shell.Launcher;

internal partial class App : Application
{
    private IContainer _container = default!;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            BindingPlugins.DataValidators.RemoveAt(0);

            var serviceCollection = new ServiceCollection();

            serviceCollection.AddSingleton<MainWindow>();
            serviceCollection.AddSingleton<MainWindowViewModel>();

            _container = serviceCollection.BuildDryIocServiceProvider().Container;

            desktop.MainWindow = _container.Resolve<MainWindow>();
        }

        base.OnFrameworkInitializationCompleted();
    }
}

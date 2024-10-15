using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using DryIoc;
using DryIoc.Microsoft.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sakuno.ING.Game;
using Sakuno.ING.ViewModels;
using Splat;

namespace Sakuno.ING.Shell;

public partial class App : Application
{
    private IHost _host = default!;
    private IContainer _container = default!;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        _host = BuildHost();
        _container = _host.Services.GetRequiredService<IContainer>();

        Locator.SetLocator(new SplatAdapter(_container));

        var resolver = Locator.CurrentMutable;
        resolver.InitializeSplat();
        resolver.InitializeReactiveUI(RegistrationNamespace.Avalonia);
        resolver.RegisterConstant(new AvaloniaActivationForViewFetcher(), typeof(IActivationForViewFetcher));
        resolver.RegisterConstant(new DataTemplateBindingHook(), typeof(IPropertyBindingHook));

        RxApp.MainThreadScheduler = AvaloniaScheduler.Instance;

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            BindingPlugins.DataValidators.RemoveAt(0);

            desktop.Startup += (sender, args) =>
            {
                _ = _host.StartAsync();
            };
            desktop.Exit += (sender, e) =>
            {
                _host.StopAsync().GetAwaiter().GetResult();
            };

            desktop.MainWindow = new MainWindow();
        }

        base.OnFrameworkInitializationCompleted();
    }

    private IHost BuildHost()
    {
        var hostBuilder = Host.CreateApplicationBuilder(Environment.GetCommandLineArgs());
        hostBuilder.ConfigureContainer(new DryIocServiceProviderFactory());

        hostBuilder.Services.AddGameServices();

        hostBuilder.Services.AddSingleton<MainWindowViewModel>();

        return hostBuilder.Build();
    }
}

using System;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using DryIoc;
using DryIoc.Microsoft.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ReactiveUI;
using Sakuno.ING.Game;
using Sakuno.ING.ViewModels;
using Splat;
using Splat.DryIoc;

namespace Sakuno.ING.Shell.Launcher;

internal partial class App : Application
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
        _container.UseDryIocDependencyResolver();

        var resolver = Locator.CurrentMutable;
        resolver.InitializeSplat();
        resolver.InitializeReactiveUI(RegistrationNamespace.Avalonia);
        resolver.RegisterConstant(new AvaloniaActivationForViewFetcher(), typeof(IActivationForViewFetcher));
        resolver.RegisterConstant(new DataTemplateBindingHook(), typeof(IPropertyBindingHook));

        RxApp.MainThreadScheduler = AvaloniaScheduler.Instance;

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            BindingPlugins.DataValidators.RemoveAt(0);

            desktop.Exit += (sender, e) =>
            {
                _host.StopAsync().GetAwaiter().GetResult();
            };

            desktop.MainWindow = _container.Resolve<MainWindow>();
        }

        base.OnFrameworkInitializationCompleted();

        _ = _host.StartAsync();
    }

    private IHost BuildHost()
    {
        var hostBuilder = Host.CreateApplicationBuilder(Environment.GetCommandLineArgs());
        hostBuilder.ConfigureContainer(new DryIocServiceProviderFactory());

        hostBuilder.Services.AddGameServices();

        hostBuilder.Services.AddSingleton<MainWindowViewModel>();
        hostBuilder.Services.AddSingleton<MainWindow>();

        return hostBuilder.Build();
    }
}

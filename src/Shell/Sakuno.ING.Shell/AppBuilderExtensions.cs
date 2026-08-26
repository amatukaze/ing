using DryIoc;
using DryIoc.Microsoft.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ReactiveUI.Reactive.Builder;
using Splat;
using Splat.Builder;
using AppBuilder = Avalonia.AppBuilder;
using SplatBuilder = Splat.Builder.AppBuilder;

namespace Sakuno.ING.Shell;

public static class AppBuilderExtensions
{
    public static AppBuilder UseShell(this AppBuilder builder)
    {
        return builder.AfterPlatformServicesSetup(_ =>
        {
            var container = BuildHost().Services.GetRequiredService<IContainer>();

            var module = new DryIocSplatModule(container);
            module.Configure(default!);

            AppLocator.CurrentMutable.RegisterConstant(container);

            var rxuiBuilder = AppLocator.CurrentMutable.CreateReactiveUIBuilder();
            rxuiBuilder.WithAvalonia();

            if (!SplatBuilder.HasBeenBuilt)
                rxuiBuilder.BuildApp();
        });

        IHost BuildHost()
        {
            var hostBuilder = Host.CreateApplicationBuilder(Environment.GetCommandLineArgs());
            hostBuilder.ConfigureContainer(new DryIocServiceProviderFactory());

            hostBuilder.Services.AddGameServices();
            hostBuilder.Services.AddViewModels();
            hostBuilder.Services.AddOverallViews();
            hostBuilder.Services.AddShellServices();

#if DEBUG
            hostBuilder.Services.AddGameDiagnostics();
#endif

            return hostBuilder.Build();
        }
    }
}

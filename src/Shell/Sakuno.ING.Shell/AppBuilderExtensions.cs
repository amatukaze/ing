using Avalonia;
using DryIoc;
using DryIoc.Microsoft.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Sakuno.ING.Shell;

public static class AppBuilderExtensions
{
    public static AppBuilder UseShell(this AppBuilder builder)
    {
        return builder.UseReactiveUIWithDIContainer(
            () => BuildHost().Services.GetRequiredService<IContainer>(),
            container =>
            {
                container.Register<IPropertyBindingHook, DataTemplateBindingHook>(Reuse.Singleton);
            },
            container => new SplatAdapter(container));

        IHost BuildHost()
        {
            var hostBuilder = Host.CreateApplicationBuilder(Environment.GetCommandLineArgs());
            hostBuilder.ConfigureContainer(new DryIocServiceProviderFactory());

            hostBuilder.Services.AddGameServices();
            hostBuilder.Services.AddViewModels();
            hostBuilder.Services.AddOverallViews();
            hostBuilder.Services.AddShellServices();

            return hostBuilder.Build();
        }
    }
}

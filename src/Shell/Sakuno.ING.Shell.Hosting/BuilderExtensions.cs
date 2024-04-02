using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Windows;

namespace Sakuno.ING.Shell.Hosting;

public static class BuilderExtensions
{
    public static IHostBuilder ConfigureShell(this IHostBuilder builder, Action<ShellBuilder>? configureBuilder = default)
    {
        var shellBuilder = new ShellBuilder();

        configureBuilder?.Invoke(shellBuilder);

        builder.ConfigureServices((context, services) =>
        {
            services.AddHostedService<ShellHostedService>();

            if (shellBuilder.ApplicationType is Type applicationType)
            {
                services.AddSingleton(applicationType);
                services.AddSingleton(typeof(Application), serviceProvider => serviceProvider.GetRequiredService(applicationType));
            }

            if (shellBuilder.MainWindowType is Type mainWindowType)
            {
                services.AddSingleton(mainWindowType);
                services.AddSingleton(typeof(IShellMainWindow), serviceProvider => serviceProvider.GetRequiredService(mainWindowType));
            }
        });

        return builder;
    }
}

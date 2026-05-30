using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Sakuno.ING.Game.Diagnostics;

internal static class ServiceRegistration
{
    [RegisterServices]
    public static void Register(IServiceCollection services)
    {
        services.AddSingleton(serviceProvider =>
        {
            var options = new DiagnosticsOptions();

            serviceProvider.GetRequiredService<IConfiguration>().GetSection("Diagnostics").Bind(options);

            return options;
        });

        services.AddHttpClient<SseDataSource>();
        services.AddSingleton<SseDataSource>();
        services.AddSingleton<IApiMessageProvider>(provider => provider.GetRequiredService<SseDataSource>());
        services.AddHostedService(provider => provider.GetRequiredService<SseDataSource>());
    }
}

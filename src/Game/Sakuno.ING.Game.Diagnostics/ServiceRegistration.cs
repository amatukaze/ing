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
            var options = new ApiDataLoaderOptions();

            serviceProvider.GetRequiredService<IConfiguration>().GetSection("ApiDataLoader").Bind(options);

            return options;
        });

        services.AddSingleton<ApiDataLoader>();
        services.AddSingleton<IApiMessageProvider>(provider => provider.GetRequiredService<ApiDataLoader>());
        services.AddHostedService(provider => provider.GetRequiredService<ApiDataLoader>());
    }
}

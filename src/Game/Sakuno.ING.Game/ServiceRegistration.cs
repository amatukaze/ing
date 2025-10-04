using Microsoft.Extensions.DependencyInjection;

namespace Sakuno.ING.Game;

internal static class ServiceRegistration
{
    [RegisterServices]
    public static void Register(IServiceCollection services)
    {
        services.AddGameProvider();
    }
}

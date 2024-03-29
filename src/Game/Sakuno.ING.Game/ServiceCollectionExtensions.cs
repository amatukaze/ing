using Microsoft.Extensions.DependencyInjection;
using Sakuno.ING.Game.Provider;
using Sakuno.ING.Game.Services;

namespace Sakuno.ING.Game;

public static class ServiceCollectionExtensions
{
    public static void AddGameServices(this IServiceCollection services)
    {
        services.AddSingleton<MasterDataService>();
        services.AddSingleton<PlayerDataService>();
        services.AddSingleton<IGameProvider, GameProvider>();
    }
}

using Microsoft.Extensions.Hosting;

namespace Sakuno.ING.Shell;

[RegisterSingleton<IHostedService>(Duplicate = DuplicateStrategy.Append)]
internal sealed class ServiceInitializer(IEnumerable<IServiceInitializable> services) : BackgroundService
{
    protected override Task ExecuteAsync(CancellationToken stoppingToken) =>
        Task.WhenAll(services.Select(service => service.InitializeAsync(stoppingToken)));
}

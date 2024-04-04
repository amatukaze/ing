using DryIoc;
using DryIoc.Microsoft.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ReactiveUI;
using Sakuno.ING.Shell;
using Sakuno.ING.Shell.Hosting;
using Splat;
using Splat.DryIoc;

var host = Host.CreateDefaultBuilder(args)
    .UseServiceProviderFactory(new DryIocServiceProviderFactory())
    .ConfigureShell(builder =>
    {
        builder.UseApplication<App>();
        builder.UseMainWindow<MainWindow>();
    })
    .ConfigureServices(services =>
    {
    })
    .Build();

var container = host.Services.GetRequiredService<IContainer>();
container.UseDryIocDependencyResolver();

var resolver = Locator.CurrentMutable;
resolver.InitializeSplat();
resolver.InitializeReactiveUI();

host.Run();

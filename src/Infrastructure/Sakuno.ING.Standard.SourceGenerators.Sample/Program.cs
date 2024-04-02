using Common;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NamespaceA;
using Sakuno.ING.Standard.SourceGenerators.Sample;

var builder = Host.CreateApplicationBuilder();

builder.Services.AddServices();

var app = builder.Build();

app.Services.GetRequiredService<Singleton>().Report();
app.Services.GetRequiredService<ISingleton>().Report();

app.Services.GetRequiredService<Singleton>().Report();
app.Services.GetRequiredService<ISingleton>().Report();

foreach (var service in app.Services.GetServices<IService>())
    service.Report();

foreach (var service in app.Services.GetServices<IService>())
    service.Report();

using System;
using System.Collections.Generic;
using DryIoc;
using Splat;
using Splat.DryIoc;

namespace Sakuno.ING.Shell.Launcher;

public class SplatAdapter(IContainer container) : IDependencyResolver
{
    private static readonly ISet<Type> FactoryTypes = new HashSet<Type>([
        typeof(Func<,>),
        typeof(Func<,,>),
    ]);

    private readonly DryIocDependencyResolver _innerAdapter = new(container);

    public object? GetService(Type? serviceType, string? contract = null)
    {
        if (serviceType is { IsGenericType: true } && FactoryTypes.Contains(serviceType.GetGenericTypeDefinition()))
            return container.Resolve(serviceType);

        return _innerAdapter.GetService(serviceType, contract);
    }

    public IEnumerable<object> GetServices(Type? serviceType, string? contract = null) => _innerAdapter.GetServices(serviceType, contract);
    public bool HasRegistration(Type? serviceType, string? contract = null) => _innerAdapter.HasRegistration(serviceType, contract);
    public void Register(Func<object?> factory, Type? serviceType, string? contract = null) => _innerAdapter.Register(factory, serviceType, contract);
    public void UnregisterCurrent(Type? serviceType, string? contract = null) => _innerAdapter.UnregisterCurrent(serviceType, contract);
    public void UnregisterAll(Type? serviceType, string? contract = null) => _innerAdapter.UnregisterAll(serviceType, contract);
    public IDisposable ServiceRegistrationCallback(Type serviceType, string? contract, Action<IDisposable> callback) => _innerAdapter.ServiceRegistrationCallback(serviceType, contract, callback);
    public void Dispose() => _innerAdapter.Dispose();
}

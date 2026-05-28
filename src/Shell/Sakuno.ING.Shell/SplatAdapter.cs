using DryIoc;
using Splat.DryIoc;

namespace Sakuno.ING.Shell;

public class SplatAdapter(IContainer container) : DryIocDependencyResolver(container)
{
    private readonly IContainer _container = container;

    private static readonly HashSet<Type> FactoryTypes =
    [
        typeof(IViewFor<>),
        typeof(Func<,>),
        typeof(Func<,,>),
    ];

    public override object? GetService(Type? serviceType, string? contract)
    {
        if (serviceType is { IsGenericType: true } && FactoryTypes.Contains(serviceType.GetGenericTypeDefinition()))
            return _container.Resolve(serviceType, contract);

        return base.GetService(serviceType, contract);
    }
}

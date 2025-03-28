namespace Sakuno.ING.Shell;

public class DependencyInjection : AvaloniaObject
{
    public static readonly AvaloniaProperty<IServiceProvider> ContainerProperty =
        AvaloniaProperty.RegisterAttached<DependencyInjection, IServiceProvider>(
            "Container", typeof(DependencyInjection), defaultValue: null!, inherits: true);

    public static IServiceProvider GetContainer(AvaloniaObject element) =>
        (IServiceProvider)element.GetValue(ContainerProperty)!;
    public static void SetContainer(AvaloniaObject element, IServiceProvider value) =>
        element.SetValue(ContainerProperty, value);
}

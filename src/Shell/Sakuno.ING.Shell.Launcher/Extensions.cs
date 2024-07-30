using Avalonia.ReactiveUI;
using ReactiveUI;
using Splat;

namespace Sakuno.ING.Shell.Launcher;

internal static class Extensions
{
    public static AppBuilder UseReactiveUI(this AppBuilder builder) =>
        builder.AfterPlatformServicesSetup(_ => Locator.RegisterResolverCallbackChanged(() =>
        {
            PlatformRegistrationManager.SetRegistrationNamespaces(RegistrationNamespace.Avalonia);
            RxApp.MainThreadScheduler = AvaloniaScheduler.Instance;
            Locator.CurrentMutable.RegisterConstant(new AvaloniaActivationForViewFetcher(), typeof(IActivationForViewFetcher));
            Locator.CurrentMutable.RegisterConstant(new DataTemplateBindingHook(), typeof(IPropertyBindingHook));
        }));
}

using Avalonia.Headless;
using Avalonia.Markup.Xaml.Styling;
using Avalonia.Styling;
using Avalonia.Themes.Fluent;
using ReactiveUI.Avalonia.Reactive;
using ReactiveUI.Reactive.Builder;

namespace Sakuno.ING.Shell.UIComponentAutomation;

public sealed class AutomationApplication : Application
{
    public override void Initialize()
    {
        RxAppBuilder.CreateReactiveUIBuilder()
            .WithAvalonia()
            .BuildApp();

        RequestedThemeVariant = ThemeVariant.Light;
        Styles.Add(new FluentTheme());
        Styles.Add(new StyleInclude(new Uri("avares://Sakuno.ING.Shell.Common/"))
        {
            Source = new Uri("avares://Sakuno.ING.Shell.Common/Themes/Generic.axaml"),
        });
    }

    internal static AppBuilder BuildAvaloniaApp() =>
        AppBuilder.Configure<AutomationApplication>()
            .UseHeadless(new()
            {
                UseHeadlessDrawing = false,
                ShouldRenderOnUIThread = true,
            })
            .UseSkia()
            .WithInterFont();
}

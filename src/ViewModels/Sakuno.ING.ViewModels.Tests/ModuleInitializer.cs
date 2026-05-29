using System.Runtime.CompilerServices;
using ReactiveUI;
using ReactiveUI.Builder;

namespace Sakuno.ING.ViewModels.Tests;

public static class ModuleInitializer
{
    [ModuleInitializer]
    public static void Initialize()
    {
        RxAppBuilder.CreateReactiveUIBuilder()
            .BuildApp();

        RxSchedulers.MainThreadScheduler = new TestScheduler();
    }
}

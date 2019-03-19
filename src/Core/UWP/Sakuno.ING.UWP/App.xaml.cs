using Sakuno.ING.Shell;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Sakuno.ING.UWP
{
    public sealed partial class App : Application
    {
        private AotCompositor compositor;

        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
        }

        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            if (compositor is null)
                compositor = new AotCompositor();
            if (e.PrelaunchActivated == false)
            {
                var initialScreen = new Grid();
                initialScreen.Loaded += (_, __) => compositor.Resolve<IShell>().Run();
                CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = false;
                Window.Current.Content = initialScreen;
                Window.Current.Activate();
            }
        }

        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            //var deferral = e.SuspendingOperation.GetDeferral();
            //deferral.Complete();
        }
    }
}

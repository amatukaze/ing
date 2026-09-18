using DryIoc;
using Splat;

namespace Sakuno.ING.Shell;

public partial class GameBrowserWindow : Window
{
    public GameBrowserWindow()
    {
        DependencyInjection.SetContainer(this, AppLocator.Current.GetService<IContainer>()!);

        InitializeComponent();
    }
}

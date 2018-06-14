using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Sakuno.ING.UWP
{
    internal sealed partial class MainView : Page
    {
        public MainView()
        {
            this.InitializeComponent();
            Window.Current.SetTitleBar(DraggableTitle);
        }

        public object MainContent
        {
            get => mainContent.Content;
            set => mainContent.Content = value;
        }
    }
}

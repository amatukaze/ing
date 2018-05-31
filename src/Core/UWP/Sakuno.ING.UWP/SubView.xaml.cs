using Windows.UI.Xaml.Controls;

namespace Sakuno.ING.UWP
{
    public sealed partial class SubView : Page
    {
        public SubView()
        {
            this.InitializeComponent();
        }
        public new object Content
        {
            get => ActualContent.Content;
            set => ActualContent.Content = value;
        }
    }
}

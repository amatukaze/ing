using Windows.UI.Xaml.Controls;

namespace Sakuno.ING.UWP
{
    public sealed partial class SubView : Page
    {
        public SubView()
        {
            this.InitializeComponent();
        }

        public object ActualContent
        {
            get => actualContent.Content;
            set => actualContent.Content = value;
        }

        public string ActualTitle
        {
            get => TitleBar.Text;
            set => TitleBar.Text = value;
        }
    }
}

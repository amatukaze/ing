namespace Sakuno.ING.Shell.Desktop
{
    internal partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public object MainContent
        {
            get => mainContent.Content;
            set => mainContent.Content = value;
        }
    }
}

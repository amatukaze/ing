using Sakuno.ING.Composition;
using System.Windows;
using System.Windows.Controls;

namespace Sakuno.ING.Shell.Desktop
{
    public class ViewPresenter : ContentControl
    {
        private string? _viewId;
        public string? ViewId
        {
            get => _viewId;
            set
            {
                _viewId = value;
                Content = Compositor.Default.ResolveViewOrDefault(value);
            }
        }

        static ViewPresenter()
        {
            HorizontalContentAlignmentProperty.OverrideMetadata(typeof(ViewPresenter), new FrameworkPropertyMetadata(HorizontalAlignment.Stretch));
            VerticalContentAlignmentProperty.OverrideMetadata(typeof(ViewPresenter), new FrameworkPropertyMetadata(VerticalAlignment.Stretch));
        }
    }
}

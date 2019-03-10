using Sakuno.ING.ViewModels.Logging;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Sakuno.ING.Views.UWP.Logging
{
    public sealed class LogFilterControl : Control
    {
        public LogFilterControl()
        {
            DefaultStyleKey = typeof(LogFilterControl);
        }

        public static readonly DependencyProperty ViewModelProperty
            = DependencyProperty.Register(nameof(ViewModel), typeof(IBindableCollection<IFilterVM>), typeof(LogFilterControl), new PropertyMetadata(null));
        public IBindableCollection<IFilterVM> ViewModel
        {
            get => (IBindableCollection<IFilterVM>)GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }
    }
}

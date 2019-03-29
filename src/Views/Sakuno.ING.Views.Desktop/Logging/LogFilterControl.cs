using System.Windows;
using System.Windows.Controls;
using Sakuno.ING.ViewModels.Logging;

namespace Sakuno.ING.Views.Desktop.Logging
{
    public sealed class LogFilterControl : Control
    {
        static LogFilterControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(LogFilterControl), new FrameworkPropertyMetadata(typeof(LogFilterControl)));
        }

        public static readonly DependencyProperty ViewModelProperty
            = DependencyProperty.Register(nameof(ViewModel), typeof(IBindableCollection<IFilterVM>), typeof(LogFilterControl));
        public IBindableCollection<IFilterVM> ViewModel
        {
            get => (IBindableCollection<IFilterVM>)GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }
    }
}

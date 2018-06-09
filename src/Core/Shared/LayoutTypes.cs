using System;
using System.Collections.Generic;
#if NET461
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
#elif WINDOWS_UWP
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;
#endif

namespace Sakuno.ING.Shell.Layout
{
    public class LayoutRoot
    {
        public LayoutWindow MainWindow { get; set; }
        public List<LayoutWindow> SubWindows { get; } = new List<LayoutWindow>();
    }

#if NET461
    [ContentProperty(nameof(Content))]
#elif WINDOWS_UWP
    [ContentProperty(Name = nameof(Content))]
#endif
    public class LayoutWindow
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public DataTemplate Content { get; set; }
    }

    public class ViewPresenter : ContentControl
    {
        public ViewPresenter()
        {
            DefaultStyleKey = typeof(ViewPresenter);
        }

        internal static readonly DependencyProperty ViewSourceProperty
            = DependencyProperty.Register(nameof(ViewSource), typeof(IReadOnlyDictionary<string, Type>), typeof(ViewPresenter), new PropertyMetadata(null, (d, _) => ((ViewPresenter)d).UpdateContent()));

        private IReadOnlyDictionary<string, Type> ViewSource => (IReadOnlyDictionary<string, Type>)GetValue(ViewSourceProperty);

        public static readonly DependencyProperty ViewIdProperty
            = DependencyProperty.Register(nameof(ViewId), typeof(string), typeof(ViewPresenter), new PropertyMetadata(null, (d, _) => ((ViewPresenter)d).UpdateContent()));
        public string ViewId
        {
            get => (string)GetValue(ViewIdProperty);
            set => SetValue(ViewIdProperty, value);
        }

        private void UpdateContent()
        {
            if (ViewSource != null && ViewSource.TryGetValue(ViewId, out Type viewType))
                Content = Activator.CreateInstance(viewType);
            else
                Content = null;
        }
    }
}

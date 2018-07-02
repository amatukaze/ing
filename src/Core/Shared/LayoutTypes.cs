using System.Collections.Generic;
using System.ComponentModel;
using Sakuno.ING.Composition;
using Sakuno.ING.Localization;
#if WPF
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
#elif WINDOWS_UWP
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Markup;
#endif

namespace Sakuno.ING.Shell.Layout
{
    public sealed class LayoutWindowList : List<LayoutWindow> { }

    public class LayoutRoot
    {
        public LayoutWindow MainWindow { get; set; }
        public LayoutWindowList SubWindows { get; } = new LayoutWindowList();
        public LayoutWindow this[string viewId]
            => SubWindows.Find(x => x.Id == viewId);
    }

#if WPF
    [ContentProperty(nameof(Content))]
#elif WINDOWS_UWP
    [ContentProperty(Name = nameof(Content))]
#endif
    public class LayoutWindow
    {
        public string Id { get; set; }
        public string Title { get; set; }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public DataTemplate Content { get; set; }
        internal object LoadContent() => Content.LoadContent();
    }

    public class ViewPresenter : ContentControl
    {
        public ViewPresenter()
        {
            HorizontalContentAlignment = HorizontalAlignment.Stretch;
            VerticalContentAlignment = VerticalAlignment.Stretch;
        }

        private string _viewId;
        public string ViewId
        {
            get => _viewId;
            set
            {
                _viewId = value;
                Content = Compositor.Default?.ResolveNamed<FrameworkElement>(value);
            }
        }
    }

    public class LocalizedTitleExtension : MarkupExtension
    {
        public LocalizedTitleExtension(string viewId) => ViewId = viewId;

#if WPF
        [ConstructorArgument("viewId")]
#endif
        public string ViewId { get; set; }

#if WPF
        public override object ProvideValue(IServiceProvider serviceProvider)
#elif WINDOWS_UWP
        protected override object ProvideValue()
#endif
            => Compositor.Static<ILocalizationService>()?.GetLocalized("ViewTitle", ViewId) ?? ViewId;
    }

    public class ViewSwitcher : Button
    {
        private bool autoContent = true;
        private string _viewId;
        public string ViewId
        {
            get => _viewId;
            set
            {
                _viewId = value;
                if (Content == null || autoContent)
                {
                    Content = Compositor.Static<ILocalizationService>()?.GetLocalized("ViewTitle", value) ?? value;
                    autoContent = false;
                }
            }
        }

        protected override void OnContentChanged(object oldContent, object newContent)
        {
            base.OnContentChanged(oldContent, newContent);
            autoContent = false;
        }

#if WPF
        protected override void OnClick()
#elif WINDOWS_UWP
        protected override void OnTapped(TappedRoutedEventArgs e)
#endif
        {
            if (ViewId != null)
                Compositor.Static<IShell>()?.SwitchWindow(ViewId);
#if WINDOWS_UWP
            e.Handled = true;
#endif
        }
    }
}

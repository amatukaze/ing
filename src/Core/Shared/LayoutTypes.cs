using System;
using System.Collections.Generic;
using Sakuno.ING.Composition;
using Sakuno.ING.Localization;
using System.ComponentModel;
#if NET461
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

#if NET461
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
            DefaultStyleKey = typeof(ViewPresenter);
        }

        internal static readonly DependencyProperty ViewSourceProperty
            = DependencyProperty.Register(nameof(ViewSource), typeof(Func<string, object>), typeof(ViewPresenter), new PropertyMetadata(null, (d, _) => ((ViewPresenter)d).UpdateContent()));

        private Func<string, object> ViewSource => (Func<string, object>)GetValue(ViewSourceProperty);

        private string _viewId;
        public string ViewId
        {
            get => _viewId;
            set
            {
                _viewId = value;
                UpdateContent();
            }
        }

        private void UpdateContent()
        {
            var obj = ViewSource?.Invoke(ViewId);
            if (obj != null)
                Content = obj;
            else
                Content = null;
        }
    }

    public class LocalizedTitleExtension : MarkupExtension
    {
        public LocalizedTitleExtension(string viewId) => ViewId = viewId;

#if NET461
        [ConstructorArgument("viewId")]
#endif
        public string ViewId { get; set; }

#if NET461
        public override object ProvideValue(IServiceProvider serviceProvider)
#elif WINDOWS_UWP
        protected override object ProvideValue()
#endif
            => Compositor.Default?.Resolve<ILocalizationService>()?.GetLocalized("ViewTitle", ViewId) ?? ViewId;
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
                    Content = Compositor.Default?.Resolve<ILocalizationService>()?.GetLocalized("ViewTitle", value) ?? value;
                    autoContent = false;
                }
            }
        }

        internal const string SwitchActionKey = "ViewSwitchAction";

        protected override void OnContentChanged(object oldContent, object newContent)
        {
            base.OnContentChanged(oldContent, newContent);
            autoContent = false;
        }

#if NET461
        protected override void OnClick()
#elif WINDOWS_UWP
        protected override void OnTapped(TappedRoutedEventArgs e)
#endif
        {
            if (ViewId != null && Application.Current.Resources[SwitchActionKey] is Action<string> action)
                action(ViewId);
#if WINDOWS_UWP
            e.Handled = true;
#endif
        }
    }
}

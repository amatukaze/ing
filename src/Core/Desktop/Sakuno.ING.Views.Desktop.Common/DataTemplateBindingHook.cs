using ReactiveUI;
using Sakuno.ING.Composition;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Sakuno.ING.Shell.Desktop
{
    [Export(typeof(IPropertyBindingHook))]
    internal sealed class DataTemplateBindingHook : IPropertyBindingHook
    {
        private DataTemplate? _defaultTemplate;
        private DataTemplate DefaultTemplate
        {
            get
            {
                if (_defaultTemplate is null)
                {
                    var factory = new FrameworkElementFactory(typeof(ViewModelViewHost));

                    factory.SetValue(Control.VerticalContentAlignmentProperty, VerticalAlignment.Stretch);
                    factory.SetValue(Control.HorizontalContentAlignmentProperty, HorizontalAlignment.Stretch);
                    factory.SetValue(Control.IsTabStopProperty, false);
                    factory.SetBinding(ViewModelViewHost.ViewModelProperty, new Binding() { Mode = BindingMode.OneWay });

                    _defaultTemplate = new DataTemplate() { VisualTree = factory };
                    _defaultTemplate.Seal();
                }

                return _defaultTemplate;
            }
        }

        public bool ExecuteHook(object? source, object target, Func<IObservedChange<object, object>[]> getCurrentViewModelProperties, Func<IObservedChange<object, object>[]>? getCurrentViewProperties, BindingDirection direction)
        {
            var viewProperties = (getCurrentViewProperties ?? throw new ArgumentNullException(nameof(getCurrentViewProperties)))();
            var lastViewProperty = viewProperties.LastOrDefault();

            if (lastViewProperty?.Sender is ItemsControl itemsControl)
            {
                if (!string.IsNullOrEmpty(itemsControl.DisplayMemberPath))
                    return true;

                if (viewProperties.Last().GetPropertyName() != nameof(ItemsControl.ItemsSource))
                    return true;

                if (itemsControl.ItemTemplate is null && itemsControl.ItemTemplateSelector is null)
                    itemsControl.ItemTemplate = DefaultTemplate;

                if (itemsControl is TabControl tabControl && tabControl.ContentTemplate is null && tabControl.ContentTemplateSelector is null)
                    tabControl.ContentTemplate = DefaultTemplate;
            }
            else if (lastViewProperty?.Sender is ContentPresenter contentPresenter)
            {
                if (viewProperties.Last().GetPropertyName() != nameof(ContentPresenter.Content))
                    return true;

                if (contentPresenter.ContentTemplate is null && contentPresenter.ContentTemplateSelector is null)
                    contentPresenter.ContentTemplate = DefaultTemplate;
            }

            return true;
        }
    }
}

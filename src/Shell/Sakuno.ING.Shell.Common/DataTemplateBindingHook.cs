using Sakuno.ING.Shell.Controls;

namespace Sakuno.ING.Shell;

public class DataTemplateBindingHook : IPropertyBindingHook
{
    private static readonly FuncDataTemplate DefaultItemTemplate = new FuncDataTemplate<object>((x, _) =>
        {
            var control = new ViewModelViewHost();
            var context = control.GetObservable(StyledElement.DataContextProperty);

            control.Bind(ViewModelViewHost.ViewModelProperty, context);
            control.HorizontalContentAlignment = HorizontalAlignment.Stretch;
            control.VerticalContentAlignment = VerticalAlignment.Stretch;
            return control;
        },
        true);

    public bool ExecuteHook(object? source, object target,
        Func<IObservedChange<object, object>[]> getCurrentViewModelProperties,
        Func<IObservedChange<object, object>[]> getCurrentViewProperties,
        BindingDirection direction)
    {
        var viewProperties = getCurrentViewProperties();
        var lastViewProperty = viewProperties.LastOrDefault();
        if (lastViewProperty?.Sender is not ItemsControl itemsControl)
            return true;

        var propertyName = viewProperties.Last().GetPropertyName();
        if (propertyName is not nameof(ItemsControl.Items) and not nameof(ItemsControl.ItemsSource))
            return true;

        if (itemsControl.ItemTemplate is not null)
            return true;

        if (itemsControl.DataTemplates is { Count: > 0 })
            return true;

        itemsControl.ItemTemplate = DefaultItemTemplate;
        return true;
    }
}

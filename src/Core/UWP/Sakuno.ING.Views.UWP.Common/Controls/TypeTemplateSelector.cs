using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;

namespace Sakuno.ING.Views.UWP.Controls
{
    [ContentProperty(Name = nameof(Selections))]
    public class TypeTemplateSelector : DataTemplateSelector
    {
        public List<TypeTemplateSelection> Selections { get; } = new List<TypeTemplateSelection>();
        public DataTemplate Fallback { get; set; }

        protected override DataTemplate SelectTemplateCore(object item)
        {
            if (item is null)
                return null;
            var typeName = item.GetType().Name;
            foreach (var s in Selections)
                if (s.TypeName == typeName)
                    return s.Template;
            return Fallback;
        }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
            => SelectTemplateCore(item);
    }

    [ContentProperty(Name = nameof(Template))]
    public class TypeTemplateSelection
    {
        // Using `Type` works in coreclr, but not .Net native
        public string TypeName { get; set; }
        public DataTemplate Template { get; set; }
    }
}

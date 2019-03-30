using System;
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
            var type = item.GetType();
            foreach (var s in Selections)
                if (s.Type == type)
                    return s.Template;
            return Fallback;
        }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
            => SelectTemplateCore(item);
    }

    [ContentProperty(Name = nameof(Template))]
    public class TypeTemplateSelection
    {
        public Type Type { get; set; }
        public DataTemplate Template { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace Sakuno.ING.Views.Desktop.Controls
{
    [ContentProperty(nameof(Selections))]
    public class TypeTemplateSelector : DataTemplateSelector
    {
        public List<TypeTemplateSelection> Selections { get; } = new List<TypeTemplateSelection>();
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is null)
                return null;
            var type = item.GetType();
            foreach (var s in Selections)
                if (s.Type == type)
                    return s.Template;
            return null;
        }
    }

    public class TypeTemplateSelection
    {
        public Type Type { get; set; }
        public DataTemplate Template { get; set; }
    }
}

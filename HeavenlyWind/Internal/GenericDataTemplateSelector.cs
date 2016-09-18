using System.Windows;
using System.Windows.Controls;

namespace Sakuno.KanColle.Amatsukaze.Internal
{
    class GenericDataTemplateSelector : DataTemplateSelector
    {
        public object AllKey { get; set; }

        public DataTemplate All { get; set; }
        public DataTemplate Specific { get; set; }

        public override DataTemplate SelectTemplate(object rpItem, DependencyObject rpContainer)
        {
            return rpItem != AllKey ? Specific : All;
        }
    }
}

using Sakuno.Collections;
using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace Sakuno.KanColle.Amatsukaze.Internal
{
    class TabContentTemplateSelector : DataTemplateSelector
    {
        static HybridDictionary<Type, DataTemplate> r_ViewTypeMap = new HybridDictionary<Type, DataTemplate>();

        static TabContentTemplateSelector()
        {
            var rAssembly = Assembly.GetExecutingAssembly();

            foreach (var rType in rAssembly.GetTypes())
            {
                var rViewInfo = rType.GetCustomAttribute<ViewInfoAttribute>();
                if (rViewInfo == null)
                    continue;

                r_ViewTypeMap.Add(rType, new DataTemplate(rType) { VisualTree = new FrameworkElementFactory(rViewInfo.ViewType) });
            }
        }

        public override DataTemplate SelectTemplate(object rpItem, DependencyObject rpContainer)
        {
            if (rpItem == null)
                return null;

            DataTemplate rResult;
            r_ViewTypeMap.TryGetValue(rpItem.GetType(), out rResult);
            return rResult;
        }
    }
}

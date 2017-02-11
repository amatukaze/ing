using Sakuno.Collections;
using Sakuno.KanColle.Amatsukaze.ViewModels;
using Sakuno.KanColle.Amatsukaze.Views;
using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

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

            var rContentPresenter = new FrameworkElementFactory(typeof(ContentPresenter));
            rContentPresenter.SetBinding(ContentControl.ContentProperty, new Binding(nameof(ToolViewModel.View)));
            r_ViewTypeMap.Add(typeof(ToolViewModel), new DataTemplate(typeof(ToolViewModel)) { VisualTree = rContentPresenter });

            r_ViewTypeMap.Add(typeof(Exception), new DataTemplate(typeof(Exception)) { VisualTree = new FrameworkElementFactory(typeof(ExceptionView)) });
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

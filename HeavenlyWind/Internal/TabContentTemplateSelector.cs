using Sakuno.Collections;
using Sakuno.KanColle.Amatsukaze.ViewModels;
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
            r_ViewTypeMap.Add(typeof(ToolWithoutScrollBarViewModel), new DataTemplate(typeof(ToolWithoutScrollBarViewModel)) { VisualTree = rContentPresenter });

            var rScrollViewer = new FrameworkElementFactory(typeof(ScrollViewer));
            rScrollViewer.SetBinding(ContentControl.ContentProperty, new Binding(nameof(ToolViewModel.View)));
            rScrollViewer.SetBinding(ScrollViewer.VerticalScrollBarVisibilityProperty, new Binding("ScrollBarVisibilities.VerticalScrollBarVisibility") { TargetNullValue = ScrollBarVisibility.Auto, FallbackValue = ScrollBarVisibility.Auto });
            rScrollViewer.SetBinding(ScrollViewer.HorizontalScrollBarVisibilityProperty, new Binding("ScrollBarVisibilities.HorizontalScrollBarVisibility") { TargetNullValue = ScrollBarVisibility.Disabled, FallbackValue = ScrollBarVisibility.Disabled });

            r_ViewTypeMap.Add(typeof(ToolWithScrollBarViewModel), new DataTemplate(typeof(ToolWithScrollBarViewModel)) { VisualTree = rScrollViewer });
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

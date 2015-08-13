using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace Sakuno.UserInterface
{
    public static class DependencyObjectExtensions
    {
        public static IEnumerable<DependencyObject> GetVisualChildren(this DependencyObject rpObject)
        {
            if (rpObject is Visual)
            {
                var rCount = VisualTreeHelper.GetChildrenCount(rpObject);
                for (var i = 0; i < rCount; i++)
                    yield return VisualTreeHelper.GetChild(rpObject, i);
            }
        }
    }
}

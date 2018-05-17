using System.Windows;

namespace Sakuno.KanColle.Amatsukaze.Controls
{
    static class Data
    {
        public static readonly DependencyProperty ContentProperty = DependencyProperty.RegisterAttached("Content", typeof(string), typeof(Data));

        public static string GetContent(DependencyObject rpObject) => (string)rpObject.GetValue(ContentProperty);
        public static void SetContent(DependencyObject rpObject, string rpValue) => rpObject.SetValue(ContentProperty, rpValue);
    }
}

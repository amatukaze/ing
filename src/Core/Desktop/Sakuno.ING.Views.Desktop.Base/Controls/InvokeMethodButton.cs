using System;
using System.Windows;
using System.Windows.Controls;

namespace Sakuno.ING.Views.Desktop.Controls
{
    public class InvokeMethodButton : Button
    {
        public static DependencyProperty MethodNameProperty
            = DependencyProperty.Register(nameof(MethodName), typeof(string), typeof(InvokeMethodButton));

        public string MethodName
        {
            get => (string)GetValue(MethodNameProperty);
            set => SetValue(MethodNameProperty, value);
        }

        public InvokeMethodButton()
        {
            SetResourceReference(StyleProperty, typeof(Button));
        }

        protected override void OnClick()
        {
            base.OnClick();

            if (MethodName == null) return;
            var method = DataContext?.GetType().GetMethod(MethodName);
            if (method == null) return;

            DesktopShellContextService.Instance.Window = Window.GetWindow(this);
            method.Invoke(DataContext, Array.Empty<object>());
            DesktopShellContextService.Instance.Window = null;
        }
    }
}

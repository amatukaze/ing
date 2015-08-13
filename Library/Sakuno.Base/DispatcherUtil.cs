using System.ComponentModel;
using System.Windows;
using System.Windows.Threading;

namespace Sakuno
{
    public static class DispatcherUtil
    {
        static Dispatcher r_UIDispatcher;
        public static Dispatcher UIDispatcher
        {
            get
            {
                if ((bool)DesignerProperties.IsInDesignModeProperty.GetMetadata(typeof(DependencyObject)).DefaultValue)
                    r_UIDispatcher = Dispatcher.CurrentDispatcher;
                return r_UIDispatcher;
            }
            set { r_UIDispatcher = value; }
        }
    }
}

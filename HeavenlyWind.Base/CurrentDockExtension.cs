using Sakuno.SystemInterop;
using Sakuno.UserInterface;
using Sakuno.UserInterface.Controls;
using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Data;
using System.Windows.Interop;
using System.Windows.Markup;

namespace Sakuno.KanColle.Amatsukaze
{
    public class CurrentDockExtension : MarkupExtension
    {
        public static bool IsAutoRotationSupported { get; }

        static CurrentDockExtension()
        {
            if (OS.IsWin8OrLater)
            {
                NativeEnums.AR_STATE rAutoRotationState;
                NativeMethods.User32.GetAutoRotationState(out rAutoRotationState);

                IsAutoRotationSupported = rAutoRotationState <= NativeEnums.AR_STATE.AR_DISABLED;
            }
        }

        public IValueConverter Converter { get; set; }
        public object ConverterParameter { get; set; }

        public override object ProvideValue(IServiceProvider rpServiceProvider)
        {
            var rResult = new MultiBinding() { Mode = BindingMode.OneWay, Converter = CoreConverter.Instance };
            rResult.Bindings.Add(new Binding() { Path = new PropertyPath(MetroWindow.ScreenOrientationProperty), RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(MetroWindow), 1) });
            rResult.Bindings.Add(new Binding("Layout.LandscapeDock") { Source = Preference.Current });
            rResult.Bindings.Add(new Binding("Layout.PortraitDock") { Source = Preference.Current });
            rResult.Bindings.Add(new Binding() { RelativeSource = RelativeSource.Self });

            if (Converter != null)
                rResult.ConverterParameter = Tuple.Create(Converter, ConverterParameter);

            return rResult.ProvideValue(rpServiceProvider);
        }

        class CoreConverter : IMultiValueConverter
        {
            public static CoreConverter Instance { get; } = new CoreConverter();

            public object Convert(object[] rpValues, Type rpTargetType, object rpParameter, CultureInfo rpCulture)
            {
                object rResult;

                if (!IsAutoRotationSupported)
                    rResult = rpValues[1];
                else
                {
                    ScreenOrientation rOrientation;

                    if (rpValues[0] != DependencyProperty.UnsetValue)
                        rOrientation = (ScreenOrientation)rpValues[0];
                    else
                    {
                        var rObject = rpValues[3] as DependencyObject;
                        var rMonitor = NativeMethods.User32.MonitorFromWindow(new WindowInteropHelper(Application.Current.MainWindow).Handle, NativeConstants.MFW.MONITOR_DEFAULTTONEAREST);
                        var rInfo = new NativeStructs.MONITORINFO() { cbSize = Marshal.SizeOf(typeof(NativeStructs.MONITORINFO)) };
                        NativeMethods.User32.GetMonitorInfo(rMonitor, ref rInfo);

                        var rWidth = rInfo.rcMonitor.Width;
                        var rHeight = rInfo.rcMonitor.Height;

                        rOrientation = rWidth > rHeight ? ScreenOrientation.Landscape : ScreenOrientation.Portrait;
                    }

                    rResult = rOrientation == ScreenOrientation.Landscape ? rpValues[1] : rpValues[2];
                }

                var rConverter = rpParameter as Tuple<IValueConverter, object>;
                return rConverter == null ? rResult : rConverter.Item1.Convert(rResult, rpTargetType, rConverter.Item2, rpCulture);
            }

            public object[] ConvertBack(object rpValue, Type[] rpTargetTypes, object rpParameter, CultureInfo rpCulture)
            {
                throw new NotSupportedException();
            }
        }
    }
}

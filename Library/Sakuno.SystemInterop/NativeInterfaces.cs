using System;
using System.Runtime.InteropServices;

namespace Sakuno.SystemInterop
{
    public static partial class NativeInterfaces
    {
        [Guid("00000000-0000-0000-c000-000000000046")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IUnknown
        {
            [PreserveSig]
            IntPtr QueryInterface(ref Guid riid, ref IntPtr pVoid);
            [PreserveSig]
            ulong AddRef();
            [PreserveSig]
            ulong Release();
        }

        [Guid("0000010d-0000-0000-C000-000000000046")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IViewObject
        {
            [PreserveSig]
            int Draw([MarshalAs(UnmanagedType.U4)] int dwDrawAspect, int lindex, IntPtr pvAspect, ref NativeStructs.DVTARGETDEVICE ptd, IntPtr hdcTargetDev, IntPtr hdcDraw, ref NativeStructs.RECT lprcBounds, ref NativeStructs.RECT lprcWBounds, IntPtr pfnContinue, IntPtr dwContinue);
        }
        [Guid("6D5140C1-7436-11CE-8034-00AA006009FA")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IServiceProvider
        {
            [PreserveSig]
            int QueryService(ref Guid guidService, ref Guid riid, [MarshalAs(UnmanagedType.Interface)] out object ppvObject);
        }
    }
}

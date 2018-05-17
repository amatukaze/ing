using Sakuno.SystemInterop;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Sakuno.KanColle.Amatsukaze.Models
{
    class ProcessInfo : ModelBase
    {
        public int ID { get; }

        public string Name { get; }

        public string Description { get; }

        public ImageSource Icon { get; }

        public ProcessInfo(Process rpProcess)
        {
            ID = rpProcess.Id;

            Name = rpProcess.ProcessName;

            var rInfo = rpProcess.MainModule.FileVersionInfo;
            if (rInfo.FileDescription != Name)
            Description = rInfo.FileDescription;

            NativeStructs.SHFILEINFO rFileInfo;
            NativeMethods.Shell32.SHGetFileInfoW(rInfo.FileName, 0, out rFileInfo, Marshal.SizeOf(typeof(NativeStructs.SHFILEINFO)), NativeEnums.SHGFI.SHGFI_ICON);

            Icon = Imaging.CreateBitmapSourceFromHIcon(rFileInfo.hIcon, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            Icon.Freeze();

            if (rFileInfo.hIcon != IntPtr.Zero)
                NativeMethods.User32.DestroyIcon(rFileInfo.hIcon);
        }
    }
}

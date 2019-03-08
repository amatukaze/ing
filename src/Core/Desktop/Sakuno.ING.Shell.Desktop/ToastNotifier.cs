using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices.ComTypes;
using Windows.UI.Notifications;
using static Sakuno.SystemLayer.NativeInterfaces;
using static Sakuno.SystemLayer.NativeStructs;

namespace Sakuno.ING.Shell.Desktop
{
    internal partial class ToastNotifier
    {
        private const string aumid = "Amatukaze.IntelligentNavalGun";

        public bool IsSupported => Environment.OSVersion.Version.Major >= 10;

        public void Initialize()
        {
            string shortcut = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Programs), "Intelligent Naval Gun.lnk");

            if (!File.Exists(shortcut))
            {
                // Find the path to the current executable
                string exePath = Process.GetCurrentProcess().MainModule.FileName;
                var newShortcut = (IShellLinkW)new CShellLink();

                // Create a shortcut to the exe
                newShortcut.SetPath(exePath);
                newShortcut.SetArguments("");

                // Open the shortcut property store, set the AppUserModelId property
                var newShortcutProperties = (IPropertyStore)newShortcut;

                using (var appId = new PROPVARIANT(aumid))
                {
                    var AUMID_KEY = new PROPERTYKEY(new Guid("9F4C2855-9F79-4B39-A8D0-E1D42DE1D5F3"), 5);

                    newShortcutProperties.SetValue(ref AUMID_KEY, appId);
                    newShortcutProperties.Commit();
                }

                // Commit the shortcut to disk
                var newShortcutSave = (IPersistFile)newShortcut;

                newShortcutSave.Save(shortcut, true);
            }

            notifier = ToastNotificationManager.CreateToastNotifier(aumid);
        }
    }
}

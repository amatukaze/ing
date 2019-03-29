using System;
using System.IO;
using Sakuno.ING.Bootstrap;

namespace Sakuno.ING
{
    public static class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            Directory.CreateDirectory("data");

            Bootstraper.InitializeFromAssemblyNames
            (
                Array.Empty<string>(),
                "Sakuno.ING.Browser.Desktop",
                "Sakuno.ING.Browser.Desktop.Cef",
                "Sakuno.ING.Core.Listener",
                "Sakuno.ING.Data",
                "Sakuno.ING.Data.Desktop",
                "Sakuno.ING.Localization.Embed",
                "Sakuno.ING.Game.Logger",
                "Sakuno.ING.Game.Logger.Migrators",
                "Sakuno.ING.Game.Models",
                "Sakuno.ING.Game.Provider",
                "Sakuno.ING.Settings.Common",
                "Sakuno.ING.Shell.Desktop",
                "Sakuno.ING.Timing.NTP",
                "Sakuno.ING.ViewModels.Logging",
                "Sakuno.ING.Views.Desktop",
                "Sakuno.ING.Views.Desktop.Base",
                "Sakuno.ING.Views.Desktop.Common"
            );

            Bootstraper.Startup();
        }
    }
}

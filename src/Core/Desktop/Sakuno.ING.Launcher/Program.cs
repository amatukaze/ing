using Sakuno.ING.Bootstrap;
using System;

namespace Sakuno.ING
{
    internal class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            Bootstraper.Initialize(new[]
            {
                "Sakuno.ING.Game.Provider",
                "Sakuno.ING.Game.Models",
                "Sakuno.ING.Shell.Desktop",
                "Sakuno.ING.ViewModels",
                "Sakuno.ING.Views.Desktop",
            });

            Bootstraper.Startup();
        }
    }
}

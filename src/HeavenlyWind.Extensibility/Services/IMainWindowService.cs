using System;
using System.Windows.Interop;

namespace Sakuno.KanColle.Amatsukaze.Extensibility.Services
{
    public interface IMainWindowService
    {
        IntPtr Handle { get; }

        void Flash(int rpCount, int rpTimeout);
    }
}

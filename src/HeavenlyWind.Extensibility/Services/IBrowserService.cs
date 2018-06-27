using System;
using System.Threading.Tasks;
using System.Windows;

namespace Sakuno.KanColle.Amatsukaze.Extensibility.Services
{
    public interface IBrowserService
    {
        IntPtr Handle { get; }

        event Action Attached;
        event EventHandler<Size> Resized;
        event Action ResizedToFitGame;

        void RegisterMessageHandler(string command, Action<string> handler);
        void RegisterAsyncMessageHandler(string command, Func<string, Task> handler);
    }
}

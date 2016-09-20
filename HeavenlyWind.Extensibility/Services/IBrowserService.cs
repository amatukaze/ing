using System;
using System.Windows;

namespace Sakuno.KanColle.Amatsukaze.Extensibility.Services
{
    public interface IBrowserService
    {
        IntPtr Handle { get; }

        event Action Attached;
        event EventHandler<Size> Resized;
        event Action ResizedToFitGame;
    }
}

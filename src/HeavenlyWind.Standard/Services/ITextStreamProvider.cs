using System;
using System.IO;

namespace Sakuno.KanColle.Amatsukaze.Services
{
    public interface ITextStreamProvider
    {
        event Action<string, TextReader> Received;
    }
}

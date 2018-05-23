using System;

namespace Sakuno.ING.Game
{
    public class SerializationError
    {
        public SerializationError(Exception exception, string path)
        {
            Exception = exception;
            Path = path;
        }

        public Exception Exception { get; }
        public string Path { get; }
    }
}

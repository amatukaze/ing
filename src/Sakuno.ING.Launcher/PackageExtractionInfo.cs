using System;

namespace Sakuno.ING
{
    struct PackageExtractionInfo
    {
        public string Filename { get; }

        public Exception Exception { get; }

        public PackageExtractionInfo(string filename, Exception exception = null)
        {
            Filename = filename;

            Exception = exception;
        }
    }
}

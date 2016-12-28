using Sakuno.KanColle.Amatsukaze.Extensibility;
using System;
using System.Linq;

namespace Sakuno.KanColle.Amatsukaze.Services.Plugins
{
    class FailureInfo
    {
        public string Filename { get; }
        public IPluginMetadata Metadata { get; }

        public string ExceptionContent { get; }

        internal FailureInfo(string rpFilename, Exception rpException)
        {
            Filename = rpFilename;
            ExceptionContent = rpException.ToString();
        }
        internal FailureInfo(string rpFilename, Exception[] rpExceptions)
        {
            Filename = rpFilename;
            ExceptionContent = rpExceptions.Select(r => r.ToString()).Join(Environment.NewLine);
        }
        internal FailureInfo(IPluginMetadata rpMetadata, Exception rpException)
        {
            Metadata = rpMetadata;
            ExceptionContent = rpException.ToString();
        }
    }
}

using System.Diagnostics;

namespace Sakuno.KanColle.Amatsukaze
{
    [DebuggerDisplay("{Dependency}: {StatusCode}")]
    struct DependencyLoadingInfo
    {
        public PackageInfo Dependency { get; }

        public StatusCode StatusCode { get; }

        public DependencyLoadingInfo(PackageInfo dependency, StatusCode statusCode)
        {
            Dependency = dependency;

            StatusCode = statusCode;
        }
    }
}

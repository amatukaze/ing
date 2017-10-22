using System.Diagnostics;

namespace HeavenlyWind
{
    [DebuggerDisplay("{Dependency}: {StatusCode}")]
    struct DependencyLoadingInfo
    {
        public DependencyInfo Dependency { get; }

        public StatusCode StatusCode { get; }

        public DependencyLoadingInfo(DependencyInfo dependency, StatusCode statusCode)
        {
            Dependency = dependency;

            StatusCode = statusCode;
        }
    }
}

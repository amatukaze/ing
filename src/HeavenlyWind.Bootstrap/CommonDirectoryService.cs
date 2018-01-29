using Sakuno.KanColle.Amatsukaze.Services;
using System.Collections.Generic;

namespace Sakuno.KanColle.Amatsukaze.Bootstrap
{
    class CommonDirectoryService : ICommonDirectoryService
    {
        public string Packages { get; }
        public string StagingPackages { get; }

        public CommonDirectoryService(IDictionary<string, object> args)
        {
            Packages = (string)args["PackageDirectory"];
            StagingPackages = (string)args["StagingPackageDirectory"];
        }
    }
}

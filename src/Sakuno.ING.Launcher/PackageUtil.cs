namespace Sakuno.ING
{
    static class PackageUtil
    {
        public const string PackageManifestFilename = "package.nuspec";

        public static string GetFilename(string uri, string relationship)
        {
            if (uri == relationship)
                return PackageManifestFilename;

            return uri.Replace('/', '\\');
        }
    }
}

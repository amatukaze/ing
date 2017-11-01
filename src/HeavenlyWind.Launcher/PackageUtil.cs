namespace HeavenlyWind
{
    static class PackageUtil
    {
        public const string ModuleManifestFilename = "package.nuspec";

        public static string GetFilenameExceptLibDirectory(string uri, string relationship)
        {
            if (uri == relationship)
                return ModuleManifestFilename;

            return uri.Replace('/', '\\');
        }

        public static string GetFilenameInLibDirectory(string uri) => uri.Substring(uri.LastIndexOf('/') + 1);
    }
}

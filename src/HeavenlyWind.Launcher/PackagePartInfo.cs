using System.IO.Compression;
using System.IO.Packaging;

namespace HeavenlyWind
{
    struct PackagePartInfo
    {
        public string Name { get; }

        public PackagePart Part { get; }
        public ZipArchiveEntry Entry { get; }

        public PackagePartInfo(string name, PackagePart part, ZipArchiveEntry entry)
        {
            Name = name;

            Part = part;
            Entry = entry;
        }
    }
}

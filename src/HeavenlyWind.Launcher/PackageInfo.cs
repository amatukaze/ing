using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Sakuno.KanColle.Amatsukaze
{
    struct PackageInfo : IEquatable<PackageInfo>
    {
        public string Id { get; }
        public string Version { get; }

        public PackageInfo(XElement dependency)
        {
            Id = dependency.Attribute("id").Value;
            Version = dependency.Attribute("version").Value;
        }
        public PackageInfo(string id, string version)
        {
            Id = id;
            Version = version;
        }

        public bool Equals(PackageInfo other) =>
            Id.Equals(other.Id, StringComparison.OrdinalIgnoreCase) &&
            Version.Equals(other.Version, StringComparison.OrdinalIgnoreCase);

        public override string ToString() => Id + " " + Version;

        public class Comparer : IEqualityComparer<PackageInfo>
        {
            public bool Equals(PackageInfo x, PackageInfo y) =>
                x.Id.Equals(y.Id, StringComparison.OrdinalIgnoreCase) &&
                x.Version.Equals(y.Version, StringComparison.OrdinalIgnoreCase);

            public int GetHashCode(PackageInfo obj) => obj.Id.GetHashCode();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace HeavenlyWind
{
    struct PackageInfo : IEquatable<PackageInfo>
    {
        public string Name { get; }
        public string Version { get; }

        public PackageInfo(XElement dependency)
        {
            Name = dependency.Attribute("id").Value;
            Version = dependency.Attribute("version").Value;
        }
        public PackageInfo(string name, string version)
        {
            Name = name;
            Version = version;
        }

        public bool Equals(PackageInfo other) =>
            Name.Equals(other.Name, StringComparison.OrdinalIgnoreCase) &&
            Version.Equals(other.Version, StringComparison.OrdinalIgnoreCase);

        public override string ToString() => Name + " " + Version;

        public class Comparer : IEqualityComparer<PackageInfo>
        {
            public bool Equals(PackageInfo x, PackageInfo y) =>
                x.Name.Equals(y.Name, StringComparison.OrdinalIgnoreCase) &&
                x.Version.Equals(y.Version, StringComparison.OrdinalIgnoreCase);

            public int GetHashCode(PackageInfo obj) => obj.Name.GetHashCode();
        }
    }
}

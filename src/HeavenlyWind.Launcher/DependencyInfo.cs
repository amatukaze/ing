using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace HeavenlyWind
{
    struct DependencyInfo : IEquatable<DependencyInfo>
    {
        public string Name { get; }
        public string Version { get; }

        public DependencyInfo(XElement dependency)
        {
            Name = dependency.Attribute("id").Value;
            Version = dependency.Attribute("version").Value;
        }
        public DependencyInfo(string name, string version)
        {
            Name = name;
            Version = version;
        }

        public bool Equals(DependencyInfo other) =>
            Name.Equals(other.Name, StringComparison.OrdinalIgnoreCase) &&
            Version.Equals(other.Version, StringComparison.OrdinalIgnoreCase);

        public override string ToString() => Name + " " + Version;

        public class Comparer : IEqualityComparer<DependencyInfo>
        {
            public bool Equals(DependencyInfo x, DependencyInfo y) =>
                x.Name.Equals(y.Name, StringComparison.OrdinalIgnoreCase) &&
                x.Version.Equals(y.Version, StringComparison.OrdinalIgnoreCase);

            public int GetHashCode(DependencyInfo obj) => obj.Name.GetHashCode();
        }
    }
}

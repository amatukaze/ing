using System;
using System.Xml.Linq;

namespace Sakuno.ING
{
    internal struct PackageInfo : IEquatable<PackageInfo>
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

        public override int GetHashCode() => Id.ToLowerInvariant().GetHashCode();

        public override string ToString() => Id + " " + Version;
    }
}

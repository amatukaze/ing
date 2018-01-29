using Sakuno.KanColle.Amatsukaze.Composition;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Sakuno.KanColle.Amatsukaze.Bootstrap
{
    class ModuleInfo : IPackage
    {
        public string Id { get; private set; }

        public string Version { get; private set; }

        public int TargetStandardVersion { get; private set; }

        public IList<string> DependsOn { get; private set; }

        public Type EntryType { get; private set; }

        public static ModuleInfo Create(string id, string version, Type type)
        {
            var targetStandardAttribute = type.GetCustomAttribute<ModuleTargetStandardVersionAttribute>();
            if (targetStandardAttribute == null)
                return null;

            var dependsOn = type.GetCustomAttributes<ModuleDependsOnAttribute>();

            return new ModuleInfo()
            {
                Id = id,
                Version = version,

                TargetStandardVersion = targetStandardAttribute.Value,

                DependsOn = dependsOn.Any() ? dependsOn.Select(r => r.Id).ToArray() : Array.Empty<string>(),

                EntryType = type,
            };
        }
    }
}

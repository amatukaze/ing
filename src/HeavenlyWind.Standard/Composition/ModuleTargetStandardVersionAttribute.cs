using System;

namespace Sakuno.KanColle.Amatsukaze.Composition
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class ModuleTargetStandardVersionAttribute : Attribute
    {
        public int Value { get; }

        public ModuleTargetStandardVersionAttribute(int standardVersion)
        {
            Value = standardVersion;
        }
    }
}

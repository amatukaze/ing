using System;

namespace Sakuno.KanColle.Amatsukaze.Composition
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public sealed class ModuleDependsOnAttribute : Attribute
    {
        public string Id { get; }

        public ModuleDependsOnAttribute(string id)
        {
            Id = id;
        }
    }
}

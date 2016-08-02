using System;

namespace Sakuno.KanColle.Amatsukaze.Internal
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    class OldPreferenceMappingAttribute : Attribute
    {
        public string Key { get; }

        public OldPreferenceMappingAttribute(string rpKey)
        {
            Key = rpKey;
        }
    }
}

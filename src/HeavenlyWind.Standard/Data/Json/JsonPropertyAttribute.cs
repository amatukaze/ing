using System;

namespace Sakuno.KanColle.Amatsukaze.Data.Json
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public sealed class JsonPropertyAttribute : Attribute
    {
        public string PropertyName { get; }

        public JsonPropertyAttribute(string propertyName) =>
            PropertyName = propertyName;
    }
}

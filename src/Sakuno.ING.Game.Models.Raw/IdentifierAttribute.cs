using System;

namespace Sakuno.ING.Game
{
    [AttributeUsage(AttributeTargets.Struct, AllowMultiple = false)]
    public sealed class IdentifierAttribute : Attribute
    {
        public IdentifierAttribute(Type underlyingType) => UnderlyingType = underlyingType;

        public Type UnderlyingType { get; }
    }
}

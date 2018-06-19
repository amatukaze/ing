using System;

namespace Sakuno.ING.Composition
{
    [AttributeUsage(AttributeTargets.Constructor, AllowMultiple = false)]
    public sealed class CompositionConstructorAttribute : Attribute
    {
    }
}

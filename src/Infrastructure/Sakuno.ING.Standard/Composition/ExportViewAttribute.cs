using System;

namespace Sakuno.ING.Composition
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class ExportViewAttribute : Attribute
    {
        public string ViewId { get; }

        public ExportViewAttribute(string viewId) => ViewId = viewId;
    }
}

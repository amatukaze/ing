using System;
using System.ComponentModel.Composition;

namespace Sakuno.KanColle.Amatsukaze.Extensibility
{
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class PluginExportAttribute : ExportAttribute
    {
        public string Name { get; }
        public string Author { get; }
        public string Version { get; }

        public string Guid { get; }

        public PluginExportAttribute(string rpName, string rpAuthor, string rpVersion, string rpGuid) : base(typeof(IPlugin))
        {
            Name = rpName;
            Author = rpAuthor;
            Version = rpVersion;

            Guid = rpGuid;
        }
    }
}

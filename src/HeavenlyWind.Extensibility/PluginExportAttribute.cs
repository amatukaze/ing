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

        public PluginExportAttribute(string name, string author, string version, string guid) : base(typeof(IPlugin))
        {
            Name = name;
            Author = author;
            Version = version;

            Guid = guid;
        }
    }
}

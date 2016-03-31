using Sakuno.KanColle.Amatsukaze.Extensibility;
using System;

namespace Sakuno.KanColle.Amatsukaze.Services.Plugins
{
    public class Plugin
    {
        IPluginMetadata r_Metadata;

        public Guid ID { get; }

        public string Name => r_Metadata.Name;
        public string Author => r_Metadata.Author;
        public string Version => r_Metadata.Version;

        internal Plugin(IPluginMetadata rpMetadata)
        {
            r_Metadata = rpMetadata;

            ID = Guid.Parse(rpMetadata.Guid);
        }
    }
}

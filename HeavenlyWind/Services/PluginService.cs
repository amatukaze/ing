using Sakuno.Collections;
using Sakuno.KanColle.Amatsukaze.Extensibility;
using Sakuno.KanColle.Amatsukaze.Services.Plugins;
using Sakuno.SystemInterop;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Sakuno.KanColle.Amatsukaze.Services
{
    public class PluginService
    {
        public static PluginService Instance { get; } = new PluginService();

        DirectoryInfo r_PluginsDirectory;

        CompositionContainer r_Container;

        HybridDictionary<Guid, Plugin> r_LoadedPlugins = new HybridDictionary<Guid, Plugin>();
        List<FailureInfo> r_Failures = new List<FailureInfo>();

        [ImportMany]
        IEnumerable<Lazy<IPlugin, IPluginMetadata>> r_PluginInfos = null;

        [ImportMany]
        IEnumerable<Lazy<IToolPane, IPluginMetadata>> r_ToolPanes = null;
        [ImportMany]
        IEnumerable<Lazy<IPreference, IPluginMetadata>> r_Preferences = null;

        PluginService() { }

        public void Initialize()
        {
            var rEntryAssembly = Assembly.GetEntryAssembly();
            var rRootDirectory = Path.GetDirectoryName(rEntryAssembly.Location);
            r_PluginsDirectory = new DirectoryInfo(Path.Combine(rRootDirectory, "Plugins"));
            if (!r_PluginsDirectory.Exists)
                return;

            var rCatalog = GenerateCatalog(rEntryAssembly);
            r_Container = new CompositionContainer(rCatalog);
            r_Container.ComposeParts(this);

            InitializePlugins();

            LoadComponents(r_ToolPanes);
            LoadComponents(r_Preferences);
        }
        AggregateCatalog GenerateCatalog(Assembly rpAssembly)
        {
            var rAggregateCatalog = new AggregateCatalog(new AssemblyCatalog(rpAssembly));

            foreach (var rPluginAssembly in r_PluginsDirectory.EnumerateFiles("*.dll", SearchOption.AllDirectories))
            {
                FileSystem.Unblock(rPluginAssembly.FullName);

                try
                {
                    var rAssemblyCatalog = new AssemblyCatalog(rPluginAssembly.FullName);
                    if (rAssemblyCatalog.Parts.Any())
                        rAggregateCatalog.Catalogs.Add(rAssemblyCatalog);
                }
                catch (BadImageFormatException e)
                {
                    r_Failures.Add(new FailureInfo(rPluginAssembly.FullName, e));
                }
                catch (FileLoadException e)
                {
                    r_Failures.Add(new FailureInfo(rPluginAssembly.FullName, e));
                }
                catch (ReflectionTypeLoadException e)
                {
                    r_Failures.Add(new FailureInfo(rPluginAssembly.FullName, e.LoaderExceptions));
                }
            }

            return rAggregateCatalog;
        }

        void InitializePlugins()
        {
            foreach (var rPluginInfo in r_PluginInfos)
            {
                Guid rGuid;
                if (!Guid.TryParse(rPluginInfo.Metadata.Guid, out rGuid))
                    continue;

                var rPlugin = new Plugin(rPluginInfo.Metadata);

                try
                {
                    rPluginInfo.Value.Initialize();

                    r_LoadedPlugins.Add(rGuid, rPlugin);
                }
                catch (Exception e)
                {
                    r_Failures.Add(new FailureInfo(rPluginInfo.Metadata, e));
                }
            }
        }

        void LoadComponents<T>(IEnumerable<Lazy<T, IPluginMetadata>> rpComponentInfos)
        {
            foreach (var rComponentInfo in rpComponentInfos)
            {
                Guid rGuid;
                if (!Guid.TryParse(rComponentInfo.Metadata.Guid, out rGuid) || !r_LoadedPlugins.ContainsKey(rGuid))
                    continue;

                try
                {
                    Cache<T>.Add(rComponentInfo.Value);
                }
                catch (Exception e)
                {
                    r_Failures.Add(new FailureInfo(rComponentInfo.Metadata, e));
                }
            }
        }

        static class Cache<T>
        {
            public static List<T> Contents { get; private set; }

            public static void Add(T rpItem)
            {
                if (Contents == null)
                    Contents = new List<T>();

                Contents.Add(rpItem);
            }
        }
    }
}

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.XPath;

namespace Sakuno.ING
{
    static class ManifestUtil
    {
        const string ManifestSchameResourceName = "Sakuno.ING.nuspec.xsd";

        static XmlReaderSettings _readerSettings = new XmlReaderSettings()
        {
            DtdProcessing = DtdProcessing.Prohibit,
            XmlResolver = null,
        };

        static ConcurrentDictionary<string, XmlSchemaSet> _schemas = new ConcurrentDictionary<string, XmlSchemaSet>(StringComparer.OrdinalIgnoreCase);
        static ConcurrentDictionary<string, XmlNamespaceManager> _namespaceManagers = new ConcurrentDictionary<string, XmlNamespaceManager>(StringComparer.OrdinalIgnoreCase);

        public static string[] TargetFrameworks = new[]
        {
            ".NETFramework4.6.1",
            ".NETFramework4.6",
            ".NETFramework4.5.2",
            ".NETFramework4.5.1",
            ".NETFramework4.5",
            ".NETFramework4.0",
            ".NETFramework3.5",
            ".NETFramework2.0",
            ".NETStandard2.0",
            ".NETStandard1.6",
            ".NETStandard1.5",
            ".NETStandard1.4",
            ".NETStandard1.3",
            ".NETStandard1.2",
            ".NETStandard1.1",
            ".NETStandard1.0",
        };

        public static XDocument Load(string filename)
        {
            try
            {
                return XDocument.Load(filename);
            }
            catch
            {
                return null;
            }
        }

        public static bool ValidateSchema(this XDocument manifest)
        {
            var schema = GetSchema(manifest);

            try
            {
                manifest.Validate(schema, (s, e) =>
                {
                    if (e.Severity == XmlSeverityType.Warning)
                        return;

                    throw e.Exception;
                });
            }
            catch { return false; }

            return true;
        }

        static string GetSchemaNamespace(XDocument manifest)
        {
            var result = "http://schemas.microsoft.com/packaging/2010/07/nuspec.xsd";

            var @namespace = manifest.Root.Name.Namespace;
            if (@namespace != null && !string.IsNullOrEmpty(@namespace.NamespaceName))
                result = @namespace.NamespaceName;

            return result;
        }

        static XmlSchemaSet GetSchema(XDocument manifest) => _schemas.GetOrAdd(GetSchemaNamespace(manifest), CreateSchemaSet);
        static XmlSchemaSet CreateSchemaSet(string schemaNamespace)
        {
            var currentAssembly = Assembly.GetExecutingAssembly();
            var result = new XmlSchemaSet();

            using (var stream = currentAssembly.GetManifestResourceStream(ManifestSchameResourceName))
            {
                var format = new StreamReader(stream).ReadToEnd();
                var content = string.Format(format, schemaNamespace);

                result.Add(schemaNamespace, XmlReader.Create(new StringReader(content), _readerSettings));
            }

            return result;
        }

        public static string GetId(this XDocument manifest)
        {
            var namespaceManager = GetNamespaceManager(manifest);
            var element = manifest.Root.XPathSelectElement("/nuspec:package/nuspec:metadata/nuspec:id", namespaceManager);

            return element.Value;
        }
        public static string GetVersion(this XDocument manifest)
        {
            var namespaceManager = GetNamespaceManager(manifest);
            var element = manifest.Root.XPathSelectElement("/nuspec:package/nuspec:metadata/nuspec:version", namespaceManager);

            return element.Value;
        }
        public static bool IsModulePackage(this XDocument manifest)
        {
            var namespaceManager = GetNamespaceManager(manifest);
            var element = manifest.Root.XPathSelectElement("/nuspec:package/nuspec:metadata/nuspec:tags", namespaceManager);
            if (element == null || element.Value == string.Empty)
                return false;

            var tags = new HashSet<string>(element.Value.Split(' '), StringComparer.OrdinalIgnoreCase);

            return tags.Contains("KanColle") && tags.Contains("ING");
        }

        public static IEnumerable<PackageInfo> EnumerateDependencies(this XDocument manifest)
        {
            var namespaceManager = GetNamespaceManager(manifest);
            var node = manifest.Root.XPathSelectElement("/nuspec:package/nuspec:metadata/nuspec:dependencies", namespaceManager);
            if (node == null)
                return Array.Empty<PackageInfo>();

            var firstChildNode = node.FirstNode as XElement;
            if (firstChildNode == null)
                return Array.Empty<PackageInfo>();

            if (firstChildNode.Name.LocalName == "dependency")
                return node.Elements().Select(r => new PackageInfo(r));

            var groups = new SortedList<string, XElement>(9, StringComparer.OrdinalIgnoreCase);

            foreach (var group in node.Elements())
                groups[group.Attribute("targetFramework")?.Value ?? string.Empty] = group;

            foreach (var targetFramework in TargetFrameworks)
                if (groups.TryGetValue(targetFramework, out var dependencies))
                    if (dependencies.FirstNode == null)
                        return Array.Empty<PackageInfo>();
                    else
                        return dependencies.Elements().Select(r => new PackageInfo(r));

            throw new InvalidOperationException();
        }

        static XmlNamespaceManager GetNamespaceManager(XDocument manifest) => _namespaceManagers.GetOrAdd(GetSchemaNamespace(manifest), CreateNamespaceManager);
        static XmlNamespaceManager CreateNamespaceManager(string schemaNamespace)
        {
            var result = new XmlNamespaceManager(new NameTable());
            result.AddNamespace("nuspec", schemaNamespace);

            return result;
        }
    }
}

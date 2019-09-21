using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace buildresw
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            static XElement ResElement(string type, string name, string value) =>
            new XElement
            (
                type,
                new XAttribute("name", name),
                new XElement("value", value)
            );

            if (args.Length < 2)
            {
                Console.WriteLine("Usage: buildresw <Input> <Output>");
                return -1;
            }
            string locales = args[0];
            string output = args[1];
            if (Directory.Exists(output))
                Directory.Delete(output, true);

            using var mapFile = File.OpenRead("fieldmap.json");
            var map = await JsonSerializer.DeserializeAsync<Dictionary<string, string>>(mapFile);

            foreach (string localeFile in Directory.EnumerateFiles(locales, "*.json"))
            {
                string localeName = Path.GetFileNameWithoutExtension(localeFile);
                string outL = Path.Combine(output, localeName);
                Directory.CreateDirectory(outL);

                using var contentFile = File.OpenRead(localeFile);
                var content = await JsonSerializer.DeserializeAsync<Dictionary<string, Dictionary<string, string>>>(contentFile);
                foreach (var entry in content)
                {
                    string resw = Path.Combine(outL, entry.Key + ".resw");
                    var r = new XElement("root");
                    var doc = new XDocument
                    (
                        new XDeclaration("1.0", "utf-8", string.Empty),
                        r
                    );

                    r.Add(ResElement("resheader", "resmimetype", "text/microsoft-resx"));
                    r.Add(ResElement("resheader", "version", "2.0"));
                    r.Add(ResElement("resheader", "reader", "System.Resources.ResXResourceReader, System.Windows.Forms, Version=2.0.3500.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"));
                    r.Add(ResElement("resheader", "writer", "System.Resources.ResXResourceWriter, System.Windows.Forms, Version=2.0.3500.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"));
                    r.Add(entry.Value.Select(x => ResElement("data", x.Key, x.Value)).ToArray());

                    doc.Save(resw);
                }

                {
                    var r = new XElement("root");
                    var doc = new XDocument
                    (
                        new XDeclaration("1.0", "utf-8", string.Empty),
                        r
                    );

                    r.Add(ResElement("resheader", "resmimetype", "text/microsoft-resx"));
                    r.Add(ResElement("resheader", "version", "2.0"));
                    r.Add(ResElement("resheader", "reader", "System.Resources.ResXResourceReader, System.Windows.Forms, Version=2.0.3500.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"));
                    r.Add(ResElement("resheader", "writer", "System.Resources.ResXResourceWriter, System.Windows.Forms, Version=2.0.3500.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"));
                    foreach (var e in map)
                    {
                        var p = e.Value.Split('/');
                        if (content.TryGetValue(p[0], out var d) &&
                            d.TryGetValue(p[1], out var v))
                            r.Add(ResElement("data", e.Key, v));
                    }

                    doc.Save(Path.Combine(outL, "Resources.resw"));
                }
            }

            return 0;
        }
    }
}

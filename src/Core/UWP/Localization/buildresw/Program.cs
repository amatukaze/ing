using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace buildresw
{
    class Program
    {
        static XElement ResElement(string type, string name, string value) =>
            new XElement
            (
                type,
                new XAttribute("name", name),
                new XElement("value", value)
            );
        static int Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Usage: buildresw <Input> <Output>");
                return -1;
            }
            string locales = args[0];
            string output = args[1];
            if (Directory.Exists(output))
                Directory.Delete(output, true);

            var mapfile = "fieldmap.json";
            var map = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(mapfile));

            foreach (string localeFile in Directory.EnumerateFiles(locales, "*.json"))
            {
                string localeName = Path.GetFileNameWithoutExtension(localeFile);
                string outL = Path.Combine(output, localeName);
                Directory.CreateDirectory(outL);

                var contentstr = File.ReadAllText(localeFile);
                var content = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(contentstr);
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

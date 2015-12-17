using Newtonsoft.Json;
using System.IO;
using System.Text;

namespace Sakuno.KanColle.Amatsukaze
{
    public partial class Preference
    {
        const string r_FilePath = @"Preferences\Preference.json";

        public static Preference Current { get; private set; }

        static JsonSerializer r_Serializer = new JsonSerializer() { Formatting = Formatting.Indented };

        public static void Load()
        {
            try
            {
                using (var rReader = new JsonTextReader(File.OpenText(r_FilePath)))
                    Current = r_Serializer.Deserialize<Preference>(rReader);
            }
            catch
            {
                Current = new Preference();
            }
        }

        public static void Save()
        {
            const string rFolder = "Preferences";

            if (!Directory.Exists(rFolder))
                Directory.CreateDirectory(rFolder);

            using (var rJsonWriter = new JsonTextWriter(new StreamWriter(r_FilePath, false, new UTF8Encoding(true))))
                r_Serializer.Serialize(rJsonWriter, Current);
        }
    }
}

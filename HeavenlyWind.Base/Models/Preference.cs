using Newtonsoft.Json;
using System.IO;
using System.Text;

namespace Sakuno.KanColle.Amatsukaze.Models
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
                using (var rReader = new StreamReader(r_FilePath))
                using (var rJsonReader = new JsonTextReader(rReader))
                    Current = r_Serializer.Deserialize<Preference>(rJsonReader);
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

            using (var rWriter = new StreamWriter(r_FilePath, false, new UTF8Encoding(true)))
            using (var rJsonWriter = new JsonTextWriter(rWriter))
                r_Serializer.Serialize(rJsonWriter, Current);
        }
    }
}

using Newtonsoft.Json;
using System;
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
                if (!File.Exists(r_FilePath))
                {
                    Current = new Preference();
                    return;
                }

                LoadCore(r_FilePath);
            }
            catch (Exception e)
            {
                try
                {
                    using (var rStreamWriter = new StreamWriter(Logger.GetNewExceptionLogFilename(), false, new UTF8Encoding(true)))
                    {
                        rStreamWriter.WriteLine("Loading preference file error.");
                        rStreamWriter.WriteLine();
                        rStreamWriter.WriteLine(e.ToString());
                    }
                }
                catch { }
            }
            finally
            {
                if (Current == null)
                    Current = new Preference();
            }
        }
        static void LoadCore(string rpPath)
        {
            using (var rReader = new JsonTextReader(File.OpenText(rpPath)))
                Current = r_Serializer.Deserialize<Preference>(rReader);
        }

        public static void Save()
        {
            const string rFolder = "Preferences";

            if (!Directory.Exists(rFolder))
                Directory.CreateDirectory(rFolder);

            const string rBackup = r_FilePath + ".bak";
            if (File.Exists(rBackup))
                File.Delete(rBackup);
            if (File.Exists(r_FilePath))
                File.Move(r_FilePath, rBackup);

            using (var rWriter = new StreamWriter(r_FilePath, false, new UTF8Encoding(true)))
            using (var rJsonWriter = new JsonTextWriter(rWriter))
                r_Serializer.Serialize(rJsonWriter, Current);
        }
    }
}

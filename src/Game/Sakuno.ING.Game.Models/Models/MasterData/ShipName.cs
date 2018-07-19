using System.Collections.Generic;
using System.Text;

namespace Sakuno.ING.Game.Models.MasterData
{
    public class ShipName : TextTranslationGroup
    {
        //private static readonly Dictionary<char, string> kanaMap = new Dictionary<char, string>
        //{

        //};

        private string _phonetic;
        public string Phonetic
        {
            get => _phonetic;
            internal set
            {
                _phonetic = value;
                //var sb = new StringBuilder(value.Length * 2);
                //foreach (var c in value)
                //    if (kanaMap.TryGetValue(c, out string roman))
                //        sb.Append(roman);
                //    else
                //        sb.Append(c);
                //sb[0] = char.ToUpperInvariant(sb[0]);

                //FullRomanization = Romanization = sb.ToString();
            }
        }
        public string AbyssalClass { get; internal set; }
        public string AbyssalFullName => Origin + AbyssalClass;
        //public string Romanization { get; private set; }
        //public string FullRomanization { get; private set; }
    }
}

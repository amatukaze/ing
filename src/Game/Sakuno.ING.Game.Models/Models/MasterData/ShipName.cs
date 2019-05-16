namespace Sakuno.ING.Game.Models.MasterData
{
    public class ShipName
    {
        private static readonly char[] identifiers
            = "イロハニホヘトチリヌルヲワカヨタレソツネナラムウヰノオクヤマケフコエテアサキユメミシヱヒモセス".ToCharArray();
        public ShipName(int id, string name, string phonetic, string abyssalClass)
        {
            FullName = new TextTranslationDescriptor(id, "ShipName", name + (abyssalClass switch
            {
                "-" => string.Empty,
                var other => other
            }));
            Phonetic = phonetic;
            if (name != null && abyssalClass != null)
            {
                AbyssalClass = abyssalClass switch
                {
                    "elite" => AbyssalShipClass.Elite,
                    "flagship" => AbyssalShipClass.Flagship,
                    _ => AbyssalShipClass.None
                };
                if (name.Contains("改"))
                {
                    name = name.Replace("改", string.Empty);
                    AbyssalClass |= AbyssalShipClass.Remodel;
                }
                if (name.Contains("後期型"))
                {
                    name = name.Replace("後期型", string.Empty);
                    AbyssalClass |= AbyssalShipClass.LateModel;
                }
                int index = name.IndexOfAny(identifiers);
                if (index != -1 && name.Length > index + 1 && name[index + 1] == '級')
                    AbyssalIdentifier = name[index];
            }
            BasicName = new TextTranslationDescriptor(id, "ShipNameBasic", name);
        }

        public ShipName() { }

        public TextTranslationDescriptor FullName { get; }
        public TextTranslationDescriptor BasicName { get; }
        public string Phonetic { get; }
        public AbyssalShipClass? AbyssalClass { get; }
        public char? AbyssalIdentifier { get; }
    }
}

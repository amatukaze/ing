namespace Sakuno.ING.Game.Models.MasterData
{
    public class ShipName : TextTranslationGroup
    {
        public ShipName() { }

        private static readonly char[] identifiers
            = "イロハニホヘトチリヌルヲワカヨタレソツネナラムウヰノオクヤマケフコエテアサキユメミシヱヒモセス".ToCharArray();
        public ShipName(string name, string phonetic, string abyssalClass)
        {
            Phonetic = phonetic;
            Origin = name + (abyssalClass switch
            {
                "-" => string.Empty,
                var other => other
            });
            if (abyssalClass != null)
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
            BasicName = name;
        }

        public string BasicName { get; }
        public string Phonetic { get; }
        public AbyssalShipClass? AbyssalClass { get; }
        public char? AbyssalIdentifier { get; }
    }
}

namespace Sakuno.KanColle.Amatsukaze.Game.Services.Quest.Parsers
{
    static class ParserCombinatorExtensions
    {
        public static Parser<T> Or<T>(this Parser<T> x, Parser<T> y) => rpInput => x(rpInput) ?? y(rpInput);
        public static Parser<T2> And<T1, T2>(this Parser<T1> x, Parser<T2> y) => rpInput => y(x(rpInput).Rest);

        public static Parser<T> AsContinuationCondition<T>(this Parser<T> rpParser) =>
            rpInput =>
            {
                var rResult = rpParser(rpInput);
                if (rResult == null)
                    return new Result<T>(default(T), rpInput);

                return new Result<T>(rResult.Value, rResult.Rest);
            };
    }
}

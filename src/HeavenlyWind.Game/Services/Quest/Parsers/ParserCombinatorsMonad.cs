using System;

namespace Sakuno.KanColle.Amatsukaze.Game.Services.Quest.Parsers
{
    static class ParserCombinatorsMonad
    {
        public static Parser<T> Where<T>(this Parser<T> rpParser, Func<T, bool> rpMatch) =>
            rpInput =>
            {
                var rResult = rpParser(rpInput);
                if (rResult == null || !rpMatch(rResult.Value))
                    return null;

                return rResult;
            };
        public static Parser<T2> Select<T1, T2>(this Parser<T1> rpParser, Func<T1, T2> rpSelector) =>
            rpInput =>
            {
                var rResult = rpParser(rpInput);
                if (rResult == null)
                    return null;

                return new Result<T2>(rpSelector(rResult.Value), rResult.Rest);
            };
        public static Parser<T2> SelectMany<T1, TIntermediate, T2>(this Parser<T1> rpParser, Func<T1, Parser<TIntermediate>> rpSelector, Func<T1, TIntermediate, T2> rpProjector) =>
            rpInput =>
            {
                var rResult = rpParser(rpInput);
                if (rResult == null)
                    return null;

                var rValue = rResult.Value;

                var rResult2 = rpSelector(rValue)(rResult.Rest);
                if (rResult2 == null)
                    return null;

                return new Result<T2>(rpProjector(rValue, rResult2.Value), rResult2.Rest);
            };
    }
}

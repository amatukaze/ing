using System;
using System.Linq;

namespace Sakuno.KanColle.Amatsukaze.Game.Services.Quest.Parsers
{
    abstract class ParserBase
    {
        protected static Parser<char> AnyChar { get; } = rpInput => rpInput?.Length > 0 ? new Result<char>(rpInput[0], rpInput.Substring(1)) : null;

        protected static Parser<char[]> Whitespace { get; } = Repeat(Char(' ').Or(Char('\t')));
        protected static Parser<int> Number { get; } = from rFirstDigit in Character(char.IsDigit)
                                                       from rDigits in Repeat(Char(char.IsDigit))
                                                       select int.Parse(new string(new[] { rFirstDigit }.Concat(rDigits).ToArray()));
        protected static Parser<string> Identifier { get; } = from rFirstCharacter in Character(char.IsLetter)
                                                              from rNextCharacters in Repeat(Char(char.IsLetterOrDigit))
                                                              select new string(new[] { rFirstCharacter }.Concat(rNextCharacters).ToArray());

        protected static Parser<char> Char(char rpChar) => from rChar in AnyChar
                                                           where rChar == rpChar
                                                           select rChar;
        protected static Parser<char> Char(Predicate<char> rpMatch) => from rChar in AnyChar
                                                                       where rpMatch(rChar)
                                                                       select rChar;

        protected static Parser<char> Character(char rpChar) => Whitespace.And(Char(rpChar));
        protected static Parser<char> Character(Predicate<char> rpMatch) => Whitespace.And(Char(rpMatch));

        protected static Parser<string> Word(string rpWord) => from rWord in Identifier
                                                               where rWord == rpWord
                                                               select rWord;

        protected static Parser<T> Succeed<T>(T rpValue) => rpInput => new Result<T>(rpValue, rpInput);

        protected static Parser<T> Option<T>(Parser<T> rpParser) where T : class => rpParser.Or(rpInput => null).Or(Succeed<T>(null));
        protected static Parser<T?> ValueTypeOption<T>(Parser<T> rpParser) where T : struct => rpParser.Select(r => new T?(r)).Or(rpInput => null).Or(Succeed<T?>(null));

        protected static Parser<T[]> Repeat<T>(Parser<T> rpParser)
        {
            Func<Parser<T>, Parser<T[]>> rCore = rpInput => from rFirst in rpParser
                                                            from rNext in Repeat(rpParser)
                                                            select Combine(rFirst, rNext);
            return rCore(rpParser).Or(Succeed(new T[0]));
        }
        static T[] Combine<T>(T x, T[] y)
        {
            var rResult = new[] { x };
            if (rResult != null)
                rResult = rResult.Concat(y).ToArray();

            return rResult;
        }
    }
}

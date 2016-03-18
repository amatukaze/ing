using Sakuno.Collections;
using Sakuno.KanColle.Amatsukaze.Game.Services.Quest.Triggers;
using System;
using System.Linq;

namespace Sakuno.KanColle.Amatsukaze.Game.Services.Quest.Parsers
{
    [TriggerName("api")]
    class ApiTriggerParserBuilder : TriggerParserBuilder
    {
        static HybridDictionary<string, Trigger> r_CachedFunctions = new HybridDictionary<string, Trigger>(StringComparer.OrdinalIgnoreCase);

        public override Parser<Trigger> Parser { get; } =
            from rApi in from rFirstCharacter in Character(char.IsLetter)
                         from rNextCharacters in Repeat(Char(char.IsLetterOrDigit).Or(Char('_')).Or(Char('/')))
                         select new string(new[] { rFirstCharacter }.Concat(rNextCharacters).ToArray())
            select r_CachedFunctions.Get(rApi, r => new ApiTrigger(r));
    }
}

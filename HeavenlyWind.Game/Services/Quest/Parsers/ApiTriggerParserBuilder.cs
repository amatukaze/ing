using Sakuno.KanColle.Amatsukaze.Game.Services.Quest.Triggers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sakuno.KanColle.Amatsukaze.Game.Services.Quest.Parsers
{
    [TriggerName("api")]
    class ApiTriggerParserBuilder : TriggerParserBuilder
    {
        static Dictionary<string, Trigger> r_CachedFunctions = new Dictionary<string, Trigger>(StringComparer.OrdinalIgnoreCase);

        public override Parser<Trigger> Parser =>
            from rApi in from rFirstCharacter in Character(char.IsLetter)
                         from rNextCharacters in Repeat(Char(char.IsLetterOrDigit).Or(Char('_')).Or(Char('/')))
                         select new string(new[] { rFirstCharacter }.Concat(rNextCharacters).ToArray())
            select r_CachedFunctions.Get(rApi, r => new ApiTrigger(r));
    }
}

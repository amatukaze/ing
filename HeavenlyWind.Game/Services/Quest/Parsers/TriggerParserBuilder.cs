using Sakuno.KanColle.Amatsukaze.Game.Services.Quest.Triggers;

namespace Sakuno.KanColle.Amatsukaze.Game.Services.Quest.Parsers
{
    abstract class TriggerParserBuilder : ParserBase
    {
        public abstract Parser<Trigger> Parser { get; }
    }
}

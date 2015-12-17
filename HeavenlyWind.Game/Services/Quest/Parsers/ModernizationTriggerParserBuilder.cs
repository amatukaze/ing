using Sakuno.KanColle.Amatsukaze.Game.Services.Quest.Triggers;

namespace Sakuno.KanColle.Amatsukaze.Game.Services.Quest.Parsers
{
    [TriggerName("modernization")]
    class ModernizationTriggerParserBuilder : TriggerParserBuilder
    {
        static ModernizationTrigger r_TriggerInstance = new ModernizationTrigger();

        public override Parser<Trigger> Parser => rpInput => new Result<Trigger>(r_TriggerInstance, rpInput);
    }
}

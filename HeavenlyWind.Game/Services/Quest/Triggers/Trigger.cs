using Sakuno.KanColle.Amatsukaze.Game.Services.Quest.Parsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Sakuno.KanColle.Amatsukaze.Game.Services.Quest.Triggers
{
    public abstract class Trigger
    {
        internal static Dictionary<string, TriggerParserBuilder> ParserBuilders { get; }

        public IObservable<object> Observable { get; protected set; }

        static Trigger()
        {
            var rAssembly = Assembly.GetExecutingAssembly();
            var rInfos = from rType in rAssembly.GetTypes()
                         where !rType.IsAbstract && rType.IsSubclassOf(typeof(TriggerParserBuilder))
                         let rAttribute = rType.GetCustomAttribute<TriggerNameAttribute>()
                         where rAttribute != null
                         select new { Name = rAttribute.Name, Parser = (TriggerParserBuilder)Activator.CreateInstance(rType) };

            ParserBuilders = rInfos.ToDictionary(r => r.Name, r => r.Parser, StringComparer.OrdinalIgnoreCase);
        }

        internal static Parser<Trigger> GetParser(string rpName)
        {
            TriggerParserBuilder rBuilder;
            if (ParserBuilders.TryGetValue(rpName, out rBuilder))
                return rBuilder.Parser;

            return rpInput => new Result<Trigger>(UnknownTrigger.Instance, rpInput);
        }
    }
}

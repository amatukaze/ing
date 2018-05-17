using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Sakuno.KanColle.Amatsukaze.Game.Services.Quest.OSS
{
    abstract class OSSQuestProgressRule
    {
        public static IDictionary<int, OSSQuestProgressRule> Maps { get; }

        static OSSQuestProgressRule()
        {
            var rAssembly = Assembly.GetExecutingAssembly();
            var rInfos = from rType in rAssembly.GetTypes()
                         where !rType.IsAbstract && rType.IsSubclassOf(typeof(OSSQuestProgressRule))
                         let rAttributes = rType.GetCustomAttributes<QuestAttribute>()
                         where rAttributes != null && rAttributes.Any()
                         let rRule = (OSSQuestProgressRule)Activator.CreateInstance(rType)
                         from rAttribute in rAttributes
                         select new { ID = rAttribute.ID, Rule = rRule };

            Maps = rInfos.ToHybridDictionary(r => r.ID, r => r.Rule);
        }

        public abstract void Register(QuestInfo rpQuest);
    }
}

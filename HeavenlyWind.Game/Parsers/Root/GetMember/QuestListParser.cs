using Newtonsoft.Json.Linq;
using Sakuno.KanColle.Amatsukaze.Game.Models;
using Sakuno.KanColle.Amatsukaze.Game.Models.Raw;
using System.Linq;

namespace Sakuno.KanColle.Amatsukaze.Game.Parsers.Root.GetMember
{
    [Api("api_get_member/questlist")]
    class QuestListParser : ApiParser<RawQuestList>
    {
        public override void Process(RawQuestList rpData)
        {
            if (rpData.Quests.Type != JTokenType.Array)
                return;

            var rQuestManager = Game.Port.Quests;

            rQuestManager.TotalCount = rpData.Count;
            rQuestManager.ExecutingCount = rpData.ExecutingCount;

            foreach (var rRawQuest in rpData.Quests.Where(r => r.Type == JTokenType.Object).Select(r => r.ToObject<RawQuest>()))
            {
                Quest rQuest;
                if (!rQuestManager.Table.TryGetValue(rRawQuest.ID, out rQuest))
                    rQuestManager.Table.Add(new Quest(rRawQuest));
                else
                    rQuest.Update(rRawQuest);
            }
        }
    }
}

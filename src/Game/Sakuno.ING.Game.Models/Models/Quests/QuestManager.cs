using Sakuno.ING.Composition;
using System;

namespace Sakuno.ING.Game.Models.Quests
{
    [Export]
    public sealed class QuestManager
    {
        private readonly IdTable<QuestId, Quest, RawQuest, QuestManager> _quests;
        public ITable<QuestId, Quest> Quests => _quests;

        public QuestManager(GameProvider provider)
        {
            _quests = new IdTable<QuestId, Quest, RawQuest, QuestManager>(this);

            provider.QuestListUpdated.Subscribe(message =>
            {
                _quests.BatchUpdate(message, removal: false);
            });

            provider.QuestCompleted.Subscribe(message =>
            {
                _quests.Remove(message);
            });
        }
    }
}

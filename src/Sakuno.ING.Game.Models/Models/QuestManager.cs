using Sakuno.ING.Game.Events;

namespace Sakuno.ING.Game.Models
{
    public class QuestManager : ITableProvider
    {
        internal QuestManager(GameListener listener)
        {
            _allQuests = new IdTable<Quest, IRawQuest>(this);
            listener.QuestUpdated.Received += QuestUpdated;
            listener.QuestCompleted.Received += msg => _allQuests.Remove(msg.Message.QuestId);
        }

        private void QuestUpdated(ITimedMessage<QuestPageUpdate> msg)
        {
            _allQuests.BatchUpdate(msg.Message.Quests, removal: false);
        }

        private readonly IdTable<Quest, IRawQuest> _allQuests;
        public ITable<Quest> AllQuests => _allQuests;

        public void Reset() => _allQuests.Clear();

        public ITable<T> TryGetTable<T>()
        {
            if (typeof(T) == typeof(Quest))
                return (ITable<T>)AllQuests;

            return null;
        }
    }
}

using Sakuno.ING.Game.Models;

namespace Sakuno.ING.Game.Events;

public interface IQuestUpdated : IIdentifiable<QuestId>
{
    string Name { get; }
    string Description { get; }
    QuestState State { get; }
}

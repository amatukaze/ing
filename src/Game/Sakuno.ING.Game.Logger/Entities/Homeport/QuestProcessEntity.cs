using System;
using Sakuno.ING.Game.Models;

namespace Sakuno.ING.Game.Logger.Entities.Homeport
{
    internal class QuestProcessEntity
    {
        public QuestId QuestId { get; set; }
        public int CounterId { get; set; }
        public int Value { get; set; }
        public bool IsActive { get; set; }
        public DateTimeOffset CheckedTime { get; set; }
    }
}

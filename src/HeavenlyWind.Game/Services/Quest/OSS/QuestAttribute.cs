using System;

namespace Sakuno.KanColle.Amatsukaze.Game.Services.Quest.OSS
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public sealed class QuestAttribute : Attribute
    {
        public int ID { get; private set; }

        public QuestAttribute(int rpID)
        {
            ID = rpID;
        }
    }
}

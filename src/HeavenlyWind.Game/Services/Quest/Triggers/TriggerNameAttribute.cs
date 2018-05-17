using System;

namespace Sakuno.KanColle.Amatsukaze.Game.Services.Quest.Triggers
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    class TriggerNameAttribute : Attribute
    {
        public string Name { get; }

        public TriggerNameAttribute(string rpName)
        {
            Name = rpName;
        }
    }
}

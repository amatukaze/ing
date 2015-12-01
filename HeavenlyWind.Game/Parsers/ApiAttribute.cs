using System;

namespace Sakuno.KanColle.Amatsukaze.Game.Parsers
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class ApiAttribute : Attribute
    {
        public string Name { get; }

        public ApiAttribute(string rpPath)
        {
            Name = rpPath;
        }
    }
}
